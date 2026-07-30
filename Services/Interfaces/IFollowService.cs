using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Follows;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IFollowService
    {
        Task<Result<object>> ToggleFollowAsync(int followerId, int followingId);
        Task<Result<FollowListDto>> GetFollowersAsync(int userId, int page, int pageSize, int? currentUserId);
        Task<Result<FollowListDto>> GetFollowingAsync(int userId, int page, int pageSize, int? currentUserId);
    }
}
