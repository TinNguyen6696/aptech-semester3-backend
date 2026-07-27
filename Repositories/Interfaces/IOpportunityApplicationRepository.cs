using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IOpportunityApplicationRepository : IGenericRepository<OpportunityApplication>
    {
        Task<bool> ExistsAsync(int opportunityId, int applicantUserId);
        Task<HashSet<int>> GetAppliedOpportunityIdsAsync(IEnumerable<int> opportunityIds, int applicantUserId);
        Task<IEnumerable<OpportunityApplication>> GetByOpportunityIdAsync(int opportunityId, int page, int pageSize);
        Task<int> CountByOpportunityIdAsync(int opportunityId);
    }
}
