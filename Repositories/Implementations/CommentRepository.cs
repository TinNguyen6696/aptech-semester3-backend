using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Comment>> GetByReferenceAsync(string referenceType, int referenceId, int page, int pageSize) =>
            await ReferenceQuery(referenceType, referenceId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByReferenceAsync(string referenceType, int referenceId) =>
            await ReferenceQuery(referenceType, referenceId).CountAsync();

        public async Task<Dictionary<int, int>> CountByReferenceIdsAsync(string referenceType, IEnumerable<int> referenceIds) =>
            await _dbSet
                .Where(c => c.ReferenceType == referenceType && referenceIds.Contains(c.ReferenceId))
                .GroupBy(c => c.ReferenceId)
                .Select(g => new { ReferenceId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ReferenceId, x => x.Count);

        public async Task<Comment?> GetByIdWithUserAsync(int id) =>
            await _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task DeleteByReferenceAsync(string referenceType, int referenceId) =>
            await _dbSet
                .Where(c => c.ReferenceType == referenceType && c.ReferenceId == referenceId)
                .ExecuteDeleteAsync();

        // Admin-only: every comment across every reference type, for moderation.
        public async Task<IEnumerable<Comment>> GetAllPagedAsync(int page, int pageSize) =>
            await _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAllAsync() =>
            await _dbSet.CountAsync();

        private IQueryable<Comment> ReferenceQuery(string referenceType, int referenceId) =>
            _dbSet
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)
                .Where(c => c.ReferenceType == referenceType && c.ReferenceId == referenceId);
    }
}
