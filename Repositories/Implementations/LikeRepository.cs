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

        public async Task<Dictionary<int, int>> CountByReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds) =>
            await _dbSet
                .Where(l => l.ReferenceType == referenceType && referenceIds.Contains(l.ReferenceId))
                .GroupBy(l => l.ReferenceId)
                .Select(g => new { ReferenceId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ReferenceId, x => x.Count);

        public async Task DeleteByReferenceAsync(string referenceType, int referenceId) =>
            await _dbSet
                .Where(l => l.ReferenceType == referenceType && l.ReferenceId == referenceId)
                .ExecuteDeleteAsync();
    }
}
