using TalentShowcase.Api.Common;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.DTOs.Videos;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class VideoService : IVideoService
    {
        private const int MaxVideosPerUser = 3;
        private const int MaxPageSize = 10;

        private readonly IVideoRepository _videoRepo;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILikeRepository _likeRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IVideoViewRepository _videoViewRepo;
        private readonly IContestEntryRepository _contestEntryRepo;
        private readonly IReportRepository _reportRepo;
        private readonly IFollowRepository _followRepo;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _context;

        public VideoService(
            IVideoRepository videoRepo,
            IFileUploadService fileUploadService,
            ILikeRepository likeRepo,
            ICommentRepository commentRepo,
            IRatingRepository ratingRepo,
            IVideoViewRepository videoViewRepo,
            IContestEntryRepository contestEntryRepo,
            IReportRepository reportRepo,
            IFollowRepository followRepo,
            INotificationService notificationService,
            IUserRepository userRepo,
            AppDbContext context)
        {
            _videoRepo = videoRepo;
            _fileUploadService = fileUploadService;
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
            _ratingRepo = ratingRepo;
            _videoViewRepo = videoViewRepo;
            _contestEntryRepo = contestEntryRepo;
            _reportRepo = reportRepo;
            _followRepo = followRepo;
            _notificationService = notificationService;
            _userRepo = userRepo;
            _context = context;
        }

        public async Task<Result<IEnumerable<VideoDto>>> GetMyVideosAsync(int userId)
        {
            var videos = (await _videoRepo.GetByUserIdAsync(userId)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id), userId);
            var dtos = videos.OrderByDescending(v => v.CreatedAt).Select(v => ToDto(v, stats[v.Id]));

            return new Result<IEnumerable<VideoDto>> { Data = dtos, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<VideoDto>> AddVideoAsync(int userId, CreateVideoRequest request)
        {
            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.Visibility!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid visibility.", StatusCode = 400 };

            var currentCount = await _videoRepo.CountByUserIdAsync(userId);
            if (currentCount >= MaxVideosPerUser)
                return new Result<VideoDto> { IsSuccess = false, Message = $"Maximum of {MaxVideosPerUser} videos reached. Delete one before adding another.", StatusCode = 400 };

            var upload = await _fileUploadService.UploadVideoAsync(request.File);
            if (!upload.IsSuccess)
                return new Result<VideoDto> { IsSuccess = false, Message = upload.Message, StatusCode = upload.StatusCode };

            var video = new Video
            {
                UserId = userId,
                Category = request.Category!.Value,
                Title = request.Title,
                Description = request.Description,
                VideoUrl = upload.Data!,
                Visibility = request.Visibility!.Value
            };

            await _videoRepo.AddAsync(video);
            await _videoRepo.SaveChangesAsync();

            if (video.Visibility == VideoVisibility.Public)
            {
                var followerIds = await _followRepo.GetAllFollowerIdsAsync(userId);
                if (followerIds.Count > 0)
                {
                    var poster = await _userRepo.GetByIdAsync(userId);
                    if (poster != null)
                        await _notificationService.CreateManyAsync(
                            followerIds,
                            $"{poster.Username} posted a new video.",
                            ReferenceTypes.Video,
                            video.Id);
                }
            }

            return new Result<VideoDto> { Data = ToDto(video, VideoStats.Empty with { IsLiked = false, IsReported = false }), IsSuccess = true, Message = "Video added successfully.", StatusCode = 201 };
        }

        public async Task<Result<VideoDto>> UpdateVideoAsync(int userId, int videoId, UpdateVideoRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);

            if (video == null || video.UserId != userId)
                return new Result<VideoDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (await _contestEntryRepo.ExistsForVideoAsync(videoId))
                return new Result<VideoDto> { IsSuccess = false, Message = "This video has been entered into a contest and can no longer be edited.", StatusCode = 400 };

            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.Visibility!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid visibility.", StatusCode = 400 };

            video.Category = request.Category!.Value;
            video.Title = request.Title;
            video.Description = request.Description;
            video.Visibility = request.Visibility!.Value;

            _videoRepo.Update(video);
            await _videoRepo.SaveChangesAsync();

            var stats = await GetStatsAsync(videoId, userId);
            return new Result<VideoDto> { Data = ToDto(video, stats), IsSuccess = true, Message = "Video updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteVideoAsync(int userId, int videoId, bool isAdmin)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);

            if (video == null || (video.UserId != userId && !isAdmin))
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            // The contest lock guards live contest history, so it does NOT apply once an admin has
            // removed the video: a removed video is already filtered out of every contest listing,
            // yet it still occupies one of the owner's MaxVideosPerUser slots (deliberate). Keeping
            // it undeletable would leave the owner permanently short a slot with no way to reclaim it.
            if (!video.IsRemovedByAdmin && await _contestEntryRepo.ExistsForVideoAsync(videoId))
                return new Result<object> { IsSuccess = false, Message = "This video has been entered into a contest and can no longer be deleted.", StatusCode = 400 };

            // Likes/Comments are polymorphic (no DB-level FK to Video), so they're cleaned up here
            // in a transaction alongside the video row. Ratings/VideoViews have a real FK to Video
            // with cascade delete, so the DB handles those automatically.
            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // A removed-by-admin video skips the lock above, so its contest entries have to be
                // cleared here — ContestEntry.VideoId is Restrict, so the video delete would fail at
                // the DB otherwise. Same two-step Restrict dance as ContestService.WithdrawEntryAsync:
                // null any Contest.WinnerEntryId pointing at these entries and flush that FIRST, then
                // remove the entries. ContestVote cascades off ContestEntry on its own.
                if (video.IsRemovedByAdmin)
                {
                    var entries = await _contestEntryRepo.GetByVideoIdWithContestAsync(videoId);
                    if (entries.Count > 0)
                    {
                        foreach (var entry in entries.Where(e => e.Contest.WinnerEntryId == e.Id))
                            entry.Contest.WinnerEntryId = null;

                        await _contestEntryRepo.SaveChangesAsync();

                        foreach (var entry in entries)
                            _contestEntryRepo.Remove(entry);

                        await _contestEntryRepo.SaveChangesAsync();
                    }
                }

                // Clear likes ON the video's comments before deleting those comments — comment-likes
                // are polymorphic too, so nothing cascades them.
                var commentIds = await _commentRepo.GetIdsByReferenceAsync(ReferenceTypes.Video, videoId);
                if (commentIds.Count > 0)
                    await _likeRepo.DeleteByReferencesAsync(ReferenceTypes.Comment, commentIds);

                await _likeRepo.DeleteByReferenceAsync(ReferenceTypes.Video, videoId);
                await _commentRepo.DeleteByReferenceAsync(ReferenceTypes.Video, videoId);

                _videoRepo.Remove(video);
                await _videoRepo.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            _fileUploadService.DeleteFile(video.VideoUrl);
            _fileUploadService.DeleteFile(video.ThumbnailUrl);

            return new Result<object> { IsSuccess = true, Message = "Video deleted successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoListDto>> GetPublicVideosAsync(TalentCategory? category, int? provinceId, SkillLevel? skillLevel, VideoSortBy sortBy, int page, int pageSize, int? currentUserId)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (skillLevel.HasValue && !Enum.IsDefined(skillLevel.Value))
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Invalid skill level.", StatusCode = 400 };

            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicAsync(category, provinceId, skillLevel);
            var videos = (await _videoRepo.GetPublicAsync(category, provinceId, skillLevel, sortBy, page, pageSize)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id), currentUserId);
            var owners = await GetOwnerFollowStateAsync(videos, currentUserId);

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(v => ToPublicDto(v, stats[v.Id], owners.FollowerCounts.GetValueOrDefault(v.UserId), owners.IsFollowing(v.UserId))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoListDto>> GetPublicVideosByUserAsync(int userId, int page, int pageSize, int? currentUserId)
        {
            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicByUserIdAsync(userId);
            var videos = (await _videoRepo.GetPublicByUserIdAsync(userId, page, pageSize)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id), currentUserId);
            var owners = await GetOwnerFollowStateAsync(videos, currentUserId);

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(v => ToPublicDto(v, stats[v.Id], owners.FollowerCounts.GetValueOrDefault(v.UserId), owners.IsFollowing(v.UserId))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoDto>> GetPublicVideoByIdAsync(int id, int? currentUserId)
        {
            var video = await _videoRepo.GetPublicByIdAsync(id);

            if (video == null)
                return new Result<PublicVideoDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var stats = await GetStatsAsync(id, currentUserId);

            var ownerFollowerCount = await _followRepo.CountFollowersAsync(video.UserId);

            bool? ownerIsFollowing = currentUserId.HasValue && currentUserId.Value != video.UserId
                ? (await _followRepo.GetAsync(currentUserId.Value, video.UserId)) != null
                : null;

            return new Result<PublicVideoDto> { Data = ToPublicDto(video, stats, ownerFollowerCount, ownerIsFollowing), IsSuccess = true, Message = "Video retrieved successfully.", StatusCode = 200 };
        }

        private async Task<VideoStats> GetStatsAsync(int videoId, int? currentUserId)
        {
            var viewCount = await _videoViewRepo.CountByVideoIdAsync(videoId);
            var likeCount = await _likeRepo.CountByReferenceAsync(ReferenceTypes.Video, videoId);
            var commentCount = await _commentRepo.CountByReferenceAsync(ReferenceTypes.Video, videoId);
            var averageRating = await _ratingRepo.GetAverageByVideoIdAsync(videoId);

            bool? isLiked = currentUserId.HasValue
                ? (await _likeRepo.GetAsync(ReferenceTypes.Video, videoId, currentUserId.Value)) != null
                : null;

            bool? isReported = currentUserId.HasValue
                ? await _reportRepo.ExistsAsync(videoId, currentUserId.Value)
                : null;

            int? myRating = currentUserId.HasValue
                ? (await _ratingRepo.GetByVideoAndUserAsync(videoId, currentUserId.Value))?.Score
                : null;

            return new VideoStats(viewCount, likeCount, commentCount, averageRating, isLiked, isReported, myRating);
        }

        private async Task<Dictionary<int, VideoStats>> GetStatsBatchAsync(IEnumerable<int> videoIds, int? currentUserId)
        {
            var ids = videoIds.ToList();

            var viewCounts = await _videoViewRepo.CountByVideoIdsAsync(ids);
            var likeCounts = await _likeRepo.CountByReferenceIdsAsync(ReferenceTypes.Video, ids);
            var commentCounts = await _commentRepo.CountByReferenceIdsAsync(ReferenceTypes.Video, ids);
            var averageRatings = await _ratingRepo.GetAverageByVideoIdsAsync(ids);

            var likedIds = currentUserId.HasValue
                ? await _likeRepo.GetLikedReferenceIdsAsync(ReferenceTypes.Video, ids, currentUserId.Value)
                : null;

            var reportedIds = currentUserId.HasValue
                ? await _reportRepo.GetReportedVideoIdsAsync(ids, currentUserId.Value)
                : null;

            var myScores = currentUserId.HasValue
                ? await _ratingRepo.GetScoresByVideoIdsAsync(ids, currentUserId.Value)
                : null;

            return ids.ToDictionary(id => id, id => new VideoStats(
                viewCounts.GetValueOrDefault(id),
                likeCounts.GetValueOrDefault(id),
                commentCounts.GetValueOrDefault(id),
                averageRatings.TryGetValue(id, out var avg) ? avg : null,
                likedIds == null ? null : likedIds.Contains(id),
                reportedIds == null ? null : reportedIds.Contains(id),
                myScores != null && myScores.TryGetValue(id, out var score) ? score : null));
        }

        // Follower count + follow state for every owner on the page, batched the same way as
        // GetStatsBatchAsync. Without this a list returns Owner.IsFollowing = null and the
        // follow button resets to "Follow" on every reload.
        private async Task<OwnerFollowState> GetOwnerFollowStateAsync(IEnumerable<Video> videos, int? currentUserId)
        {
            var ownerIds = videos.Select(v => v.UserId).Distinct().ToList();

            var followerCounts = await _followRepo.CountFollowersBatchAsync(ownerIds);

            // null = anonymous viewer, no follow button either way.
            var followingIds = currentUserId.HasValue
                ? await _followRepo.GetFollowingIdsAsync(currentUserId.Value, ownerIds)
                : null;

            return new OwnerFollowState(followerCounts, followingIds, currentUserId);
        }

        private static VideoDto ToDto(Video video, VideoStats stats) => new VideoDto
        {
            Id = video.Id,
            Category = video.Category,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            Visibility = video.Visibility,
            IsRemovedByAdmin = video.IsRemovedByAdmin,
            ViewCount = stats.ViewCount,
            LikeCount = stats.LikeCount,
            IsLiked = stats.IsLiked,
            CommentCount = stats.CommentCount,
            AverageRating = stats.AverageRating,
            MyRating = stats.MyRating,
            IsReported = stats.IsReported,
            CreatedAt = video.CreatedAt
        };

        private static PublicVideoDto ToPublicDto(Video video, VideoStats stats, int? ownerFollowerCount = null, bool? ownerIsFollowing = null) => new PublicVideoDto
        {
            Id = video.Id,
            Category = video.Category,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            ViewCount = stats.ViewCount,
            LikeCount = stats.LikeCount,
            IsLiked = stats.IsLiked,
            CommentCount = stats.CommentCount,
            AverageRating = stats.AverageRating,
            MyRating = stats.MyRating,
            IsReported = stats.IsReported,
            CreatedAt = video.CreatedAt,
            Owner = new VideoOwnerDto
            {
                Id = video.User.Id,
                Username = video.User.Username,
                ProfileImageUrl = video.User.Profile?.ProfileImageUrl,
                PrimaryCategory = video.User.Profile?.PrimaryCategory,
                FollowerCount = ownerFollowerCount,
                IsFollowing = ownerIsFollowing,
                SkillLevel = video.User.Profile?.SkillLevel
            }
        };

        private record VideoStats(int ViewCount, int LikeCount, int CommentCount, double? AverageRating, bool? IsLiked, bool? IsReported, int? MyRating)
        {
            public static readonly VideoStats Empty = new(0, 0, 0, null, null, null, null);
        }

        private record OwnerFollowState(Dictionary<int, int> FollowerCounts, HashSet<int>? FollowingIds, int? CurrentUserId)
        {
            // null on my own video too — same convention as GetPublicVideoByIdAsync, so the
            // client's single rule stays "show the button only when IsFollowing != null".
            public bool? IsFollowing(int ownerId) =>
                FollowingIds == null || ownerId == CurrentUserId ? null : FollowingIds.Contains(ownerId);
        }
    }
}
