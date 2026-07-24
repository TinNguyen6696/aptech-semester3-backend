using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IRatingRepository : IGenericRepository<Rating>
    {
        Task<Rating?> GetByVideoAndUserAsync(int videoId, int userId);
        Task<double?> GetAverageByVideoIdAsync(int videoId);
        Task<Dictionary<int, double>> GetAverageByVideoIdsAsync(IEnumerable<int> videoIds);
        Task<Dictionary<int, int>> GetScoresByVideoIdsAsync(IEnumerable<int> videoIds, int userId);
    }
}