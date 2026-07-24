using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IContestRepository : IGenericRepository<Contest>
    {
        Task<IEnumerable<Contest>> GetPublicAsync(TalentCategory? category, int page, int pageSize);
        Task<int> CountPublicAsync(TalentCategory? category);
        Task<int> CountEndedAsync();
        Task<IEnumerable<Contest>> GetEndedUnprocessedAsync();
    }
}