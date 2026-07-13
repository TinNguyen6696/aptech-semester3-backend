using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(AppDbContext context) : base(context) { }

        public async Task<Rating?> GetByVideoAndUserAsync(int videoId, int userId) =>
            await _dbSet.FirstOrDefaultAsync(r => r.VideoId == videoId && r.UserId == userId);

        public async Task<double?> GetAverageByVideoIdAsync(int videoId)
        {
            var ratings = _dbSet.Where(r => r.VideoId == videoId);
            return await ratings.AnyAsync() ? await ratings.AverageAsync(r => r.Score) : null;
        }

        public async Task<Dictionary<int, double>> GetAverageByVideoIdsAsync(IEnumerable<int> videoIds) =>
            await _dbSet
                .Where(r => videoIds.Contains(r.VideoId))
                .GroupBy(r => r.VideoId)
                .Select(g => new { VideoId = g.Key, Average = g.Average(r => r.Score) })
                .ToDictionaryAsync(x => x.VideoId, x => x.Average);
    }
}