using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class LikeRepository : GenericRepository<Like>, ILikeRepository
    {
        public LikeRepository(AppDbContext context) : base(context) { }

        public async Task<Like?> GetAsync(string referenceType, int referenceId, int userId) =>
            await _dbSet.FirstOrDefaultAsync(l =>
                l.ReferenceType == referenceType && l.ReferenceId == referenceId && l.UserId == userId);

        public async Task<int> CountByReferenceAsync(string referenceType, int referenceId) =>
            await _dbSet.CountAsync(l => l.ReferenceType == referenceType && l.ReferenceId == referenceId);

        public async Task<int> CountByReferenceTypeAsync(string referenceType) =>
            await _dbSet.CountAsync(l => l.ReferenceType == referenceType);

        public async Task<Dictionary<int, int>> CountByReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds) =>
            await _dbSet
                .Where(l => l.ReferenceType == referenceType && referenceIds.Contains(l.ReferenceId))
                .GroupBy(l => l.ReferenceId)
                .Select(g => new { ReferenceId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ReferenceId, x => x.Count);

        // Returns the subset of referenceIds that this specific user has liked — used to
        // compute a per-item "isLiked" flag for the current viewer across a page of results
        // in one batched query, instead of one existence check per item.
        public async Task<HashSet<int>> GetLikedReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds, int userId)
        {
            var liked = await _dbSet
                .Where(l => l.ReferenceType == referenceType && l.UserId == userId && referenceIds.Contains(l.ReferenceId))
                .Select(l => l.ReferenceId)
                .ToListAsync();

            return liked.ToHashSet();
        }

        public async Task DeleteByReferenceAsync(string referenceType, int referenceId) =>
            await _dbSet
                .Where(l => l.ReferenceType == referenceType && l.ReferenceId == referenceId)
                .ExecuteDeleteAsync();

        // Batch variant: delete all likes pointing at any of the given referenceIds — used when
        // a parent (video/post) is deleted to clear the likes on all of its child comments at once.
        public async Task DeleteByReferencesAsync(string referenceType, IEnumerable<int> referenceIds) =>
            await _dbSet
                .Where(l => l.ReferenceType == referenceType && referenceIds.Contains(l.ReferenceId))
                .ExecuteDeleteAsync();
    }
}
