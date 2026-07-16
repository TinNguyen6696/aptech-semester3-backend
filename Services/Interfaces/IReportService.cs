using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Reports;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IReportService
    {
        Task<Result<object>> CreateReportAsync(int userId, int videoId, CreateReportRequest request);
        Task<Result<ReportListDto>> GetReportsAsync(ReportStatus? status, int page, int pageSize);
        Task<Result<ReportDto>> GetReportByIdAsync(int id);
        Task<Result<ReportDto>> UpdateReportStatusAsync(int adminUserId, int reportId, UpdateReportStatusRequest request);
    }
}