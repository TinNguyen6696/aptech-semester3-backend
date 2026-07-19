using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(AppDbContext context) : base(context) { }

        public async Task<Follow?> GetAsync(int followerId, int followingId) =>
            await _dbSet.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        public async Task<int> CountFollowersAsync(int userId) =>
            await _dbSet.CountAsync(f => f.FollowingId == userId);

        public async Task<int> CountFollowingAsync(int userId) =>
            await _dbSet.CountAsync(f => f.FollowerId == userId);

        public async Task<Dictionary<int, int>> CountFollowersBatchAsync(IEnumerable<int> userIds) =>
            await _dbSet
                .Where(f => userIds.Contains(f.FollowingId))
                .GroupBy(f => f.FollowingId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

        public async Task<HashSet<int>> GetFollowingIdsAsync(int followerId, IEnumerable<int> targetIds)
        {
            var following = await _dbSet
                .Where(f => f.FollowerId == followerId && targetIds.Contains(f.FollowingId))
                .Select(f => f.FollowingId)
                .ToListAsync();

            return following.ToHashSet();
        }

        public async Task<IEnumerable<Follow>> GetFollowersAsync(int userId, int page, int pageSize) =>
            await _dbSet
                .Include(f => f.Follower)
                    .ThenInclude(u => u.Profile)
                .Where(f => f.FollowingId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<IEnumerable<Follow>> GetFollowingAsync(int userId, int page, int pageSize) =>
            await _dbSet
                .Include(f => f.Following)
                    .ThenInclude(u => u.Profile)
                .Where(f => f.FollowerId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }
}
