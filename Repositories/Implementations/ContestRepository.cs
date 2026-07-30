using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class ContestRepository : GenericRepository<Contest>, IContestRepository
    {
        // Same reasoning as UserRepository: DB default collation is accent-sensitive, this one
        // folds Vietnamese diacritics so "phuong" matches "Phương".
        private const string AccentInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AI";

        public ContestRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Contest>> GetPublicAsync(TalentCategory? category, string? search, int page, int pageSize) =>
            await Query(category, search)
                .OrderByDescending(c => c.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicAsync(TalentCategory? category, string? search) =>
            await Query(category, search).CountAsync();

        public async Task<int> CountEndedAsync() =>
            await _dbSet.CountAsync(c => c.EndDate < DateTime.UtcNow);

        public async Task<IEnumerable<Contest>> GetEndedUnprocessedAsync() =>
            await _dbSet
                .Where(c => c.EndDate < DateTime.UtcNow && c.WinnerAnnouncedAt == null)
                .ToListAsync();

        private IQueryable<Contest> Query(TalentCategory? category, string? search)
        {
            var query = _dbSet.AsQueryable();

            if (category.HasValue)
                query = query.Where(c => c.Category == category.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    EF.Functions.Collate(c.Title, AccentInsensitiveCollation).Contains(search) ||
                    (c.Description != null && EF.Functions.Collate(c.Description, AccentInsensitiveCollation).Contains(search)));

            return query;
        }
    }
}