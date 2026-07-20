using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Communities;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface ICommunityService
    {
        Task<Result<IEnumerable<CommunityDto>>> GetCommunitiesAsync();
        Task<Result<CommunityDto>> GetCommunityByIdAsync(int id);
        Task<Result<CommunityPostListDto>> GetCommunityPostsAsync(int communityId, int page, int pageSize, int? currentUserId);
        Task<Result<CommunityPostDto>> GetCommunityPostByIdAsync(int postId, int? currentUserId);
        Task<Result<CommunityPostDto>> AddCommunityPostAsync(int userId, int communityId, CreateCommunityPostRequest request);
        Task<Result<CommunityPostDto>> UpdateCommunityPostAsync(int userId, int postId, UpdateCommunityPostRequest request);
        Task<Result<object>> DeleteCommunityPostAsync(int userId, int postId, bool isAdmin);
    }
}