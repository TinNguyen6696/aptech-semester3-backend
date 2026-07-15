using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IOpportunityRepository : IGenericRepository<Opportunity>
    {
        Task<IEnumerable<Opportunity>> GetPublicAsync(TalentCategory? category, int? provinceId, int page, int pageSize);
        Task<int> CountPublicAsync(TalentCategory? category, int? provinceId);
        Task<Opportunity?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Opportunity>> GetByUserIdAsync(int userId, int page, int pageSize);
        Task<int> CountByUserIdAsync(int userId);
    }
}