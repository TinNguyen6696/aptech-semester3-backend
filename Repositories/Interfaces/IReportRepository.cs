using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<bool> ExistsAsync(int videoId, int reporterUserId);
        Task<HashSet<int>> GetReportedVideoIdsAsync(IEnumerable<int> videoIds, int userId);
        Task<IEnumerable<Report>> GetPagedAsync(ReportStatus? status, int page, int pageSize);
        Task<int> CountAsync(ReportStatus? status);
        Task<Report?> GetByIdWithDetailsAsync(int id);
    }
}