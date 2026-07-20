using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class CommunityPostRepository : GenericRepository<CommunityPost>, ICommunityPostRepository
    {
        public CommunityPostRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CommunityPost>> GetByCommunityIdAsync(int communityId, int page, int pageSize) =>
            await _dbSet
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Where(p => p.CommunityId == communityId && p.User.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByCommunityIdAsync(int communityId) =>
            await _dbSet.CountAsync(p => p.CommunityId == communityId && p.User.IsActive);

        public async Task<CommunityPost?> GetByIdWithUserAsync(int id) =>
            await _dbSet
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<CommunityPost?> GetPublicByIdAsync(int id) =>
            await _dbSet
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(p => p.Id == id && p.User.IsActive);

        public async Task<Dictionary<int, int>> CountByCommunityIdsAsync(IEnumerable<int> communityIds) =>
            await _dbSet
                .Where(p => communityIds.Contains(p.CommunityId) && p.User.IsActive)
                .GroupBy(p => p.CommunityId)
                .Select(g => new { CommunityId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CommunityId, x => x.Count);

        public async Task<int> CountAllAsync() =>
            await _dbSet.CountAsync();
    }
}