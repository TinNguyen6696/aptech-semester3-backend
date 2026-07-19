using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(int videoId, int reporterUserId) =>
            await _dbSet.AnyAsync(r => r.VideoId == videoId && r.ReporterUserId == reporterUserId);

        public async Task<HashSet<int>> GetReportedVideoIdsAsync(IEnumerable<int> videoIds, int userId)
        {
            var reported = await _dbSet
                .Where(r => r.ReporterUserId == userId && videoIds.Contains(r.VideoId))
                .Select(r => r.VideoId)
                .ToListAsync();

            return reported.ToHashSet();
        }

        public async Task<IEnumerable<Report>> GetPagedAsync(ReportStatus? status, int page, int pageSize) =>
            await Query(status)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAsync(ReportStatus? status) =>
            await Query(status).CountAsync();

        public async Task<Report?> GetByIdWithDetailsAsync(int id) =>
            await _dbSet
                .Include(r => r.Video)
                .Include(r => r.ReporterUser)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.ReviewedByUser)
                    .ThenInclude(u => u!.Profile)
                .FirstOrDefaultAsync(r => r.Id == id);

        private IQueryable<Report> Query(ReportStatus? status)
        {
            var query = _dbSet
                .Include(r => r.Video)
                .Include(r => r.ReporterUser)
                    .ThenInclude(u => u.Profile)
                .Include(r => r.ReviewedByUser)
                    .ThenInclude(u => u!.Profile)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            return query;
        }
    }
}