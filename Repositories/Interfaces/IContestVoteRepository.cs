using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IContestVoteRepository : IGenericRepository<ContestVote>
    {
        Task<ContestVote?> GetAsync(int contestEntryId, int userId);
        Task<int> CountByEntryIdAsync(int contestEntryId);
        Task<Dictionary<int, int>> CountByEntryIdsAsync(IEnumerable<int> contestEntryIds);
    }
}