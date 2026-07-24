using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class ContestRepository : GenericRepository<Contest>, IContestRepository
    {
        public ContestRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Contest>> GetPublicAsync(TalentCategory? category, int page, int pageSize) =>
            await Query(category)
                .OrderByDescending(c => c.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicAsync(TalentCategory? category) =>
            await Query(category).CountAsync();

        public async Task<int> CountEndedAsync() =>
            await _dbSet.CountAsync(c => c.EndDate < DateTime.UtcNow);

        private IQueryable<Contest> Query(TalentCategory? category)
        {
            var query = _dbSet.AsQueryable();

            if (category.HasValue)
                query = query.Where(c => c.Category == category.Value);

            return query;
        }
    }
}