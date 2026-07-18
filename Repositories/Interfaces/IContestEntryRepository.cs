using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IContestEntryRepository : IGenericRepository<ContestEntry>
    {
        Task<bool> ExistsForVideoAsync(int videoId);
        Task<ContestEntry?> GetAsync(int contestId, int videoId);
        Task<ContestEntry?> GetByIdWithDetailsAsync(int entryId);
        Task<IEnumerable<ContestEntry>> GetByContestIdAsync(int contestId, int page, int pageSize);
        Task<int> CountByContestIdAsync(int contestId);
        Task<Dictionary<int, int>> CountByContestIdsAsync(IEnumerable<int> contestIds);
        Task<IEnumerable<ContestEntry>> GetByEntrantUserIdAsync(int userId, int page, int pageSize);
        Task<int> CountByEntrantUserIdAsync(int userId);
    }
}