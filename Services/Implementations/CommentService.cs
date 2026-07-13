using TalentShowcase.Api.Common;
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
        private readonly IUserRepository _userRepo;

        public CommentService(ICommentRepository commentRepo, IVideoRepository videoRepo, IUserRepository userRepo)
        {
            _commentRepo = commentRepo;
            _videoRepo = videoRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<CommentListDto>> GetVideoCommentsAsync(int videoId, int page, int pageSize)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<CommentListDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<CommentListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<CommentListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _commentRepo.CountByReferenceAsync(ReferenceTypes.Video, videoId);
            var comments = await _commentRepo.GetByReferenceAsync(ReferenceTypes.Video, videoId, page, pageSize);

            var result = new CommentListDto
            {
                Comments = comments.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<CommentListDto> { Data = result, IsSuccess = true, Message = "Comments retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<CommentDto>> AddVideoCommentAsync(int userId, int videoId, CreateCommentRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<CommentDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            var comment = new Comment
            {
                UserId = userId,
                ReferenceType = ReferenceTypes.Video,
                ReferenceId = videoId,
                Content = request.Content,
                User = user!
            };

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return new Result<CommentDto> { Data = ToDto(comment), IsSuccess = true, Message = "Comment added successfully.", StatusCode = 201 };
        }

        public async Task<Result<CommentDto>> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest request)
        {
            var comment = await _commentRepo.GetByIdWithUserAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return new Result<CommentDto> { IsSuccess = false, Message = "Comment not found.", StatusCode = 404 };

            comment.Content = request.Content;
            _commentRepo.Update(comment);
            await _commentRepo.SaveChangesAsync();

            return new Result<CommentDto> { Data = ToDto(comment), IsSuccess = true, Message = "Comment updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteCommentAsync(int userId, int commentId, bool isAdmin)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment == null || (comment.UserId != userId && !isAdmin))
                return new Result<object> { IsSuccess = false, Message = "Comment not found.", StatusCode = 404 };

            _commentRepo.Remove(comment);
            await _commentRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Comment deleted successfully.", StatusCode = 200 };
        }

        private static CommentDto ToDto(Comment comment) => new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
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