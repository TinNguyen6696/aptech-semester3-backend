using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<Follow?> GetAsync(int followerId, int followingId);
        Task<int> CountFollowersAsync(int userId);
        Task<int> CountFollowingAsync(int userId);
        Task<Dictionary<int, int>> CountFollowersBatchAsync(IEnumerable<int> userIds);
        Task<HashSet<int>> GetFollowingIdsAsync(int followerId, IEnumerable<int> targetIds);
        Task<IEnumerable<Follow>> GetFollowersAsync(int userId, int page, int pageSize);
        Task<List<int>> GetAllFollowerIdsAsync(int userId);
        Task<IEnumerable<Follow>> GetFollowingAsync(int userId, int page, int pageSize);
    }
}
