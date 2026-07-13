using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<Follow?> GetAsync(int followerId, int followingId);
        Task<int> CountFollowersAsync(int userId);
        Task<int> CountFollowingAsync(int userId);
        Task<IEnumerable<Follow>> GetFollowersAsync(int userId, int page, int pageSize);
        Task<IEnumerable<Follow>> GetFollowingAsync(int userId, int page, int pageSize);
    }
}
