using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class ContestVoteRepository : GenericRepository<ContestVote>, IContestVoteRepository
    {
        public ContestVoteRepository(AppDbContext context) : base(context) { }

        public async Task<ContestVote?> GetAsync(int contestEntryId, int userId) =>
            await _dbSet.FirstOrDefaultAsync(v => v.ContestEntryId == contestEntryId && v.UserId == userId);

        public async Task<int> CountByEntryIdAsync(int contestEntryId) =>
            await _dbSet.CountAsync(v => v.ContestEntryId == contestEntryId);

        public async Task<Dictionary<int, int>> CountByEntryIdsAsync(IEnumerable<int> contestEntryIds) =>
            await _dbSet
                .Where(v => contestEntryIds.Contains(v.ContestEntryId))
                .GroupBy(v => v.ContestEntryId)
                .Select(g => new { ContestEntryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ContestEntryId, x => x.Count);

        // The subset of entryIds this user has voted for — used to compute a per-entry "isVoted"
        // flag across a page of entries in one batched query (mirrors LikeRepo's isLiked pattern).
        public async Task<HashSet<int>> GetVotedEntryIdsAsync(IEnumerable<int> contestEntryIds, int userId)
        {
            var voted = await _dbSet
                .Where(v => v.UserId == userId && contestEntryIds.Contains(v.ContestEntryId))
                .Select(v => v.ContestEntryId)
                .ToListAsync();

            return voted.ToHashSet();
        }
    }
}