using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class OpportunityRepository : GenericRepository<Opportunity>, IOpportunityRepository
    {
        public OpportunityRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Opportunity>> GetPublicAsync(TalentCategory? category, int? provinceId, int page, int pageSize) =>
            await Query(category, provinceId, null)
                .Where(o => o.PostedByUser.IsActive)
                .OrderByDescending(o => o.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicAsync(TalentCategory? category, int? provinceId) =>
            await Query(category, provinceId, null).Where(o => o.PostedByUser.IsActive).CountAsync();

        public async Task<Opportunity?> GetByIdWithDetailsAsync(int id) =>
            await _dbSet
                .Include(o => o.PostedByUser)
                    .ThenInclude(u => u.Profile)
                .Include(o => o.Province)
                .FirstOrDefaultAsync(o => o.Id == id && o.PostedByUser.IsActive);

        public async Task<IEnumerable<Opportunity>> GetByUserIdAsync(int userId, int page, int pageSize) =>
            await Query(null, null, userId)
                .OrderByDescending(o => o.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByUserIdAsync(int userId) =>
            await Query(null, null, userId).CountAsync();

        // Public "view this recruiter's postings" (e.g. from their profile page) — unlike
        // GetByUserIdAsync (used for the recruiter's own "mine" list), this excludes banned
        // posters, same as GetPublicAsync/GetByIdWithDetailsAsync.
        public async Task<IEnumerable<Opportunity>> GetPublicByUserIdAsync(int userId, int page, int pageSize) =>
            await Query(null, null, userId)
                .Where(o => o.PostedByUser.IsActive)
                .OrderByDescending(o => o.PostedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicByUserIdAsync(int userId) =>
            await Query(null, null, userId).Where(o => o.PostedByUser.IsActive).CountAsync();

        private IQueryable<Opportunity> Query(TalentCategory? category, int? provinceId, int? postedByUserId)
        {
            var query = _dbSet
                .Include(o => o.PostedByUser)
                    .ThenInclude(u => u.Profile)
                .Include(o => o.Province)
                .AsQueryable();

            if (category.HasValue)
                query = query.Where(o => o.Category == category.Value);

            if (provinceId.HasValue)
                query = query.Where(o => o.ProvinceId == provinceId.Value);

            if (postedByUserId.HasValue)
                query = query.Where(o => o.PostedByUserId == postedByUserId.Value);

            return query;
        }
    }
}