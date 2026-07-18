using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class ContestEntryRepository : GenericRepository<ContestEntry>, IContestEntryRepository
    {
        public ContestEntryRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsForVideoAsync(int videoId) =>
            await _dbSet.AnyAsync(e => e.VideoId == videoId);

        public async Task<ContestEntry?> GetAsync(int contestId, int videoId) =>
            await _dbSet.FirstOrDefaultAsync(e => e.ContestId == contestId && e.VideoId == videoId);

        public async Task<ContestEntry?> GetByIdWithDetailsAsync(int entryId) =>
            await _dbSet
                .Include(e => e.Contest)
                .Include(e => e.Video)
                    .ThenInclude(v => v.User)
                        .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(e => e.Id == entryId);

        // Ordered by vote count descending (the leaderboard) at the DB level, BEFORE pagination —
        // sorting after Skip/Take would only rank within a page and put the real winner on page 2.
        // CreatedAt is the tie-breaker so equal-vote entries have a stable order.
        public async Task<IEnumerable<ContestEntry>> GetByContestIdAsync(int contestId, int page, int pageSize) =>
            await _dbSet
                .Include(e => e.Video)
                    .ThenInclude(v => v.User)
                        .ThenInclude(u => u.Profile)
                .Where(e => e.ContestId == contestId)
                .OrderByDescending(e => _context.ContestVotes.Count(cv => cv.ContestEntryId == e.Id))
                .ThenByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByContestIdAsync(int contestId) =>
            await _dbSet.CountAsync(e => e.ContestId == contestId);

        public async Task<Dictionary<int, int>> CountByContestIdsAsync(IEnumerable<int> contestIds) =>
            await _dbSet
                .Where(e => contestIds.Contains(e.ContestId))
                .GroupBy(e => e.ContestId)
                .Select(g => new { ContestId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ContestId, x => x.Count);

        // A user "enters" a contest by submitting one of their videos, so entrant = entry.Video.UserId.
        // Includes Contest + Video for the "My Entries" tab; newest submission first.
        public async Task<IEnumerable<ContestEntry>> GetByEntrantUserIdAsync(int userId, int page, int pageSize) =>
            await _dbSet
                .Include(e => e.Contest)
                .Include(e => e.Video)
                .Where(e => e.Video.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByEntrantUserIdAsync(int userId) =>
            await _dbSet.CountAsync(e => e.Video.UserId == userId);
    }
}