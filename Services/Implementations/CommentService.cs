using TalentShowcase.Api.Common;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private const int MaxPageSize = 10;

        private readonly ICommentRepository _commentRepo;
        private readonly IVideoRepository _videoRepo;
        private readonly ICommunityPostRepository _communityPostRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;

        public CommentService(
            ICommentRepository commentRepo,
            IVideoRepository videoRepo,
            ICommunityPostRepository communityPostRepo,
            ILikeRepository likeRepo,
            IUserRepository userRepo,
            INotificationService notificationService,
            AppDbContext context)
        {
            _commentRepo = commentRepo;
            _videoRepo = videoRepo;
            _communityPostRepo = communityPostRepo;
            _likeRepo = likeRepo;
            _userRepo = userRepo;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<Result<CommentListDto>> GetVideoCommentsAsync(int videoId, int page, int pageSize, int? currentUserId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<CommentListDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            return await GetCommentsAsync(ReferenceTypes.Video, videoId, page, pageSize, currentUserId);
        }

        public async Task<Result<CommentDto>> AddVideoCommentAsync(int userId, int videoId, CreateCommentRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<CommentDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var result = await AddCommentAsync(userId, ReferenceTypes.Video, videoId, request);

            if (result.IsSuccess && video.UserId != userId)
                await NotifyAsync(video.UserId, userId, "commented on your video", ReferenceTypes.Video, videoId);

            return result;
        }

        public async Task<Result<CommentListDto>> GetCommunityPostCommentsAsync(int postId, int page, int pageSize, int? currentUserId)
        {
            var post = await _communityPostRepo.GetByIdAsync(postId);
            if (post == null)
                return new Result<CommentListDto> { IsSuccess = false, Message = "Post not found.", StatusCode = 404 };

            return await GetCommentsAsync(ReferenceTypes.CommunityPost, postId, page, pageSize, currentUserId);
        }

        public async Task<Result<CommentDto>> AddCommunityPostCommentAsync(int userId, int postId, CreateCommentRequest request)
        {
            var post = await _communityPostRepo.GetByIdAsync(postId);
            if (post == null)
                return new Result<CommentDto> { IsSuccess = false, Message = "Post not found.", StatusCode = 404 };

            var result = await AddCommentAsync(userId, ReferenceTypes.CommunityPost, postId, request);

            if (result.IsSuccess && post.UserId != userId)
                await NotifyAsync(post.UserId, userId, "commented on your post", ReferenceTypes.CommunityPost, postId);

            return result;
        }

        public async Task<Result<CommentDto>> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest request)
        {
            var comment = await _commentRepo.GetByIdWithUserAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return new Result<CommentDto> { IsSuccess = false, Message = "Comment not found.", StatusCode = 404 };

            comment.Content = request.Content;
            _commentRepo.Update(comment);
            await _commentRepo.SaveChangesAsync();

            var likeCount = await _likeRepo.CountByReferenceAsync(ReferenceTypes.Comment, commentId);
            var isLiked = (await _likeRepo.GetAsync(ReferenceTypes.Comment, commentId, userId)) != null;

            return new Result<CommentDto> { Data = ToDto(comment, likeCount, isLiked), IsSuccess = true, Message = "Comment updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteCommentAsync(int userId, int commentId, bool isAdmin)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment == null || (comment.UserId != userId && !isAdmin))
                return new Result<object> { IsSuccess = false, Message = "Comment not found.", StatusCode = 404 };

            // Clear the comment's own likes (polymorphic, no FK) alongside the comment row, atomically.
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await _likeRepo.DeleteByReferenceAsync(ReferenceTypes.Comment, commentId);

            _commentRepo.Remove(comment);
            await _commentRepo.SaveChangesAsync();

            await transaction.CommitAsync();

            return new Result<object> { IsSuccess = true, Message = "Comment deleted successfully.", StatusCode = 200 };
        }

        private async Task<Result<CommentListDto>> GetCommentsAsync(string referenceType, int referenceId, int page, int pageSize, int? currentUserId)
        {
            if (page < 1)
                return new Result<CommentListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<CommentListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _commentRepo.CountByReferenceAsync(referenceType, referenceId);
            var comments = (await _commentRepo.GetByReferenceAsync(referenceType, referenceId, page, pageSize)).ToList();

            var commentIds = comments.Select(c => c.Id).ToList();
            var likeCounts = await _likeRepo.CountByReferenceIdsAsync(ReferenceTypes.Comment, commentIds);
            var likedIds = currentUserId.HasValue
                ? await _likeRepo.GetLikedReferenceIdsAsync(ReferenceTypes.Comment, commentIds, currentUserId.Value)
                : null;

            var result = new CommentListDto
            {
                Comments = comments.Select(c => ToDto(
                    c,
                    likeCounts.GetValueOrDefault(c.Id),
                    likedIds == null ? null : likedIds.Contains(c.Id))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<CommentListDto> { Data = result, IsSuccess = true, Message = "Comments retrieved successfully.", StatusCode = 200 };
        }

        private async Task<Result<CommentDto>> AddCommentAsync(int userId, string referenceType, int referenceId, CreateCommentRequest request)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            var comment = new Comment
            {
                UserId = userId,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Content = request.Content,
                User = user!
            };

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return new Result<CommentDto> { Data = ToDto(comment, 0, false), IsSuccess = true, Message = "Comment added successfully.", StatusCode = 201 };
        }

        private async Task NotifyAsync(int recipientUserId, int actorUserId, string action, string referenceType, int referenceId)
        {
            var actor = await _userRepo.GetByIdAsync(actorUserId);
            if (actor == null)
                return;

            await _notificationService.CreateAsync(recipientUserId, $"{actor.Username} {action}.", referenceType, referenceId);
        }

        private static CommentDto ToDto(Comment comment, int likeCount, bool? isLiked) => new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            LikeCount = likeCount,
            IsLiked = isLiked,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Author = new CommentAuthorDto
            {
                Id = comment.User.Id,
                Username = comment.User.Username,
                ProfileImageUrl = comment.User.Profile?.ProfileImageUrl
            }
        };
    }
}