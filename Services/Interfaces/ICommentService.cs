using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentListDto>> GetVideoCommentsAsync(int videoId, int page, int pageSize);
        Task<Result<CommentDto>> AddVideoCommentAsync(int userId, int videoId, CreateCommentRequest request);
        Task<Result<CommentListDto>> GetCommunityPostCommentsAsync(int postId, int page, int pageSize);
        Task<Result<CommentDto>> AddCommunityPostCommentAsync(int userId, int postId, CreateCommentRequest request);
        Task<Result<CommentDto>> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest request);
        Task<Result<object>> DeleteCommentAsync(int userId, int commentId, bool isAdmin);
    }
}