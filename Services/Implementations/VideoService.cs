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
        private readonly AppDbContext _context;

        public VideoService(
            IVideoRepository videoRepo,
            IFileUploadService fileUploadService,
            ILikeRepository likeRepo,
            ICommentRepository commentRepo,
            IRatingRepository ratingRepo,
            IVideoViewRepository videoViewRepo,
            IContestEntryRepository contestEntryRepo,
            AppDbContext context)
        {
            _videoRepo = videoRepo;
            _fileUploadService = fileUploadService;
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
            _ratingRepo = ratingRepo;
            _videoViewRepo = videoViewRepo;
            _contestEntryRepo = contestEntryRepo;
            _context = context;
        }

        public async Task<Result<IEnumerable<VideoDto>>> GetMyVideosAsync(int userId)
        {
            var videos = (await _videoRepo.GetByUserIdAsync(userId)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id));
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

            return new Result<VideoDto> { Data = ToDto(video, VideoStats.Empty), IsSuccess = true, Message = "Video added successfully.", StatusCode = 201 };
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

            var stats = await GetStatsAsync(videoId);
            return new Result<VideoDto> { Data = ToDto(video, stats), IsSuccess = true, Message = "Video updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteVideoAsync(int userId, int videoId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);

            if (video == null || video.UserId != userId)
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (await _contestEntryRepo.ExistsForVideoAsync(videoId))
                return new Result<object> { IsSuccess = false, Message = "This video has been entered into a contest and can no longer be deleted.", StatusCode = 400 };

            // Likes/Comments are polymorphic (no DB-level FK to Video), so they're cleaned up here
            // in a transaction alongside the video row. Ratings/VideoViews have a real FK to Video
            // with cascade delete, so the DB handles those automatically.
            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
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

        public async Task<Result<PublicVideoListDto>> GetPublicVideosAsync(TalentCategory? category, VideoSortBy sortBy, int page, int pageSize)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicAsync(category);
            var videos = (await _videoRepo.GetPublicAsync(category, sortBy, page, pageSize)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id));

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(v => ToPublicDto(v, stats[v.Id])),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoListDto>> GetPublicVideosByUserAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicByUserIdAsync(userId);
            var videos = (await _videoRepo.GetPublicByUserIdAsync(userId, page, pageSize)).ToList();
            var stats = await GetStatsBatchAsync(videos.Select(v => v.Id));

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(v => ToPublicDto(v, stats[v.Id])),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoDto>> GetPublicVideoByIdAsync(int id)
        {
            var video = await _videoRepo.GetPublicByIdAsync(id);

            if (video == null)
                return new Result<PublicVideoDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var stats = await GetStatsAsync(id);
            return new Result<PublicVideoDto> { Data = ToPublicDto(video, stats), IsSuccess = true, Message = "Video retrieved successfully.", StatusCode = 200 };
        }

        private async Task<VideoStats> GetStatsAsync(int videoId)
        {
            var viewCount = await _videoViewRepo.CountByVideoIdAsync(videoId);
            var likeCount = await _likeRepo.CountByReferenceAsync(ReferenceTypes.Video, videoId);
            var commentCount = await _commentRepo.CountByReferenceAsync(ReferenceTypes.Video, videoId);
            var averageRating = await _ratingRepo.GetAverageByVideoIdAsync(videoId);

            return new VideoStats(viewCount, likeCount, commentCount, averageRating);
        }

        private async Task<Dictionary<int, VideoStats>> GetStatsBatchAsync(IEnumerable<int> videoIds)
        {
            var ids = videoIds.ToList();

            var viewCounts = await _videoViewRepo.CountByVideoIdsAsync(ids);
            var likeCounts = await _likeRepo.CountByReferenceIdsAsync(ReferenceTypes.Video, ids);
            var commentCounts = await _commentRepo.CountByReferenceIdsAsync(ReferenceTypes.Video, ids);
            var averageRatings = await _ratingRepo.GetAverageByVideoIdsAsync(ids);

            return ids.ToDictionary(id => id, id => new VideoStats(
                viewCounts.GetValueOrDefault(id),
                likeCounts.GetValueOrDefault(id),
                commentCounts.GetValueOrDefault(id),
                averageRatings.TryGetValue(id, out var avg) ? avg : null));
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
            ViewCount = stats.ViewCount,
            LikeCount = stats.LikeCount,
            CommentCount = stats.CommentCount,
            AverageRating = stats.AverageRating,
            CreatedAt = video.CreatedAt
        };

        private static PublicVideoDto ToPublicDto(Video video, VideoStats stats) => new PublicVideoDto
        {
            Id = video.Id,
            Category = video.Category,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            ViewCount = stats.ViewCount,
            LikeCount = stats.LikeCount,
            CommentCount = stats.CommentCount,
            AverageRating = stats.AverageRating,
            CreatedAt = video.CreatedAt,
            Owner = new VideoOwnerDto
            {
                Id = video.User.Id,
                Username = video.User.Username,
                ProfileImageUrl = video.User.Profile?.ProfileImageUrl,
                PrimaryCategory = video.User.Profile?.PrimaryCategory,
                SkillLevel = video.User.Profile?.SkillLevel
            }
        };

        private record VideoStats(int ViewCount, int LikeCount, int CommentCount, double? AverageRating)
        {
            public static readonly VideoStats Empty = new(0, 0, 0, null);
        }
    }
}
