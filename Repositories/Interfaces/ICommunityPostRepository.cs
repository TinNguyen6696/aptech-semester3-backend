using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface ICommunityPostRepository : IGenericRepository<CommunityPost>
    {
        Task<IEnumerable<CommunityPost>> GetByCommunityIdAsync(int communityId, int page, int pageSize);
        Task<int> CountByCommunityIdAsync(int communityId);
        Task<CommunityPost?> GetByIdWithUserAsync(int id);
        Task<Dictionary<int, int>> CountByCommunityIdsAsync(IEnumerable<int> communityIds);
        Task<int> CountAllAsync();
    }
}