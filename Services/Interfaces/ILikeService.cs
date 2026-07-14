using TalentShowcase.Api.Common;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface ILikeService
    {
        Task<Result<object>> ToggleVideoLikeAsync(int userId, int videoId);
        Task<Result<object>> ToggleCommunityPostLikeAsync(int userId, int postId);
    }
}