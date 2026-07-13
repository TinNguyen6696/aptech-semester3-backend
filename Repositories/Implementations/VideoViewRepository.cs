using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class VideoViewRepository : GenericRepository<VideoView>, IVideoViewRepository
    {
        public VideoViewRepository(AppDbContext context) : base(context) { }

        public async Task<int> CountByVideoIdAsync(int videoId) =>
            await _dbSet.CountAsync(v => v.VideoId == videoId);

        public async Task<Dictionary<int, int>> CountByVideoIdsAsync(IEnumerable<int> videoIds) =>
            await _dbSet
                .Where(v => videoIds.Contains(v.VideoId))
                .GroupBy(v => v.VideoId)
                .Select(g => new { VideoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VideoId, x => x.Count);
    }
}