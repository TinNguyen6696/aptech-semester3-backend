using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class OpportunityApplicationRepository : GenericRepository<OpportunityApplication>, IOpportunityApplicationRepository
    {
        public OpportunityApplicationRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(int opportunityId, int applicantUserId) =>
            await _dbSet.AnyAsync(a => a.OpportunityId == opportunityId && a.ApplicantUserId == applicantUserId);

        public async Task<IEnumerable<OpportunityApplication>> GetByOpportunityIdAsync(int opportunityId, int page, int pageSize) =>
            await _dbSet
                .Include(a => a.ApplicantUser)
                    .ThenInclude(u => u.Profile)
                .Where(a => a.OpportunityId == opportunityId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByOpportunityIdAsync(int opportunityId) =>
            await _dbSet.CountAsync(a => a.OpportunityId == opportunityId);

        public async Task<HashSet<int>> GetAppliedOpportunityIdsAsync(IEnumerable<int> opportunityIds, int applicantUserId)
        {
            var applied = await _dbSet
                .Where(a => a.ApplicantUserId == applicantUserId && opportunityIds.Contains(a.OpportunityId))
                .Select(a => a.OpportunityId)
                .ToListAsync();

            return applied.ToHashSet();
        }
    }
}
