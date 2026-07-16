using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Reports;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class ReportService : IReportService
    {
        private const int MaxPageSize = 10;

        private readonly IReportRepository _reportRepo;
        private readonly IVideoRepository _videoRepo;

        public ReportService(IReportRepository reportRepo, IVideoRepository videoRepo)
        {
            _reportRepo = reportRepo;
            _videoRepo = videoRepo;
        }

        public async Task<Result<object>> CreateReportAsync(int userId, int videoId, CreateReportRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (video.UserId == userId)
                return new Result<object> { IsSuccess = false, Message = "You cannot report your own video.", StatusCode = 400 };

            if (await _reportRepo.ExistsAsync(videoId, userId))
                return new Result<object> { IsSuccess = false, Message = "You have already reported this video.", StatusCode = 400 };

            var report = new Report
            {
                VideoId = videoId,
                ReporterUserId = userId,
                Description = request.Description,
                Status = ReportStatus.Pending
            };

            await _reportRepo.AddAsync(report);
            await _reportRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Report submitted successfully.", StatusCode = 201 };
        }

        public async Task<Result<ReportListDto>> GetReportsAsync(ReportStatus? status, int page, int pageSize)
        {
            if (status.HasValue && !Enum.IsDefined(status.Value))
                return new Result<ReportListDto> { IsSuccess = false, Message = "Invalid status.", StatusCode = 400 };

            if (page < 1)
                return new Result<ReportListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<ReportListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _reportRepo.CountAsync(status);
            var reports = await _reportRepo.GetPagedAsync(status, page, pageSize);

            var result = new ReportListDto
            {
                Reports = reports.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<ReportListDto> { Data = result, IsSuccess = true, Message = "Reports retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ReportDto>> GetReportByIdAsync(int id)
        {
            var report = await _reportRepo.GetByIdWithDetailsAsync(id);
            if (report == null)
                return new Result<ReportDto> { IsSuccess = false, Message = "Report not found.", StatusCode = 404 };

            return new Result<ReportDto> { Data = ToDto(report), IsSuccess = true, Message = "Report retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<ReportDto>> UpdateReportStatusAsync(int adminUserId, int reportId, UpdateReportStatusRequest request)
        {
            var report = await _reportRepo.GetByIdWithDetailsAsync(reportId);
            if (report == null)
                return new Result<ReportDto> { IsSuccess = false, Message = "Report not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.Status!.Value))
                return new Result<ReportDto> { IsSuccess = false, Message = "Invalid status.", StatusCode = 400 };

            report.Status = request.Status!.Value;
            report.ReviewedByUserId = adminUserId;
            report.ReviewedAt = DateTime.UtcNow;

            _reportRepo.Update(report);
            await _reportRepo.SaveChangesAsync();

            // Re-fetch: ReviewedByUser navigation on the in-memory `report` is stale (it was
            // loaded before ReviewedByUserId changed), same reasoning as the Opportunity
            // province-swap fix — a fresh query correctly resolves the new relation.
            var updated = await _reportRepo.GetByIdWithDetailsAsync(reportId);
            return new Result<ReportDto> { Data = ToDto(updated!), IsSuccess = true, Message = "Report status updated successfully.", StatusCode = 200 };
        }

        private static ReportDto ToDto(Report report) => new ReportDto
        {
            Id = report.Id,
            VideoId = report.VideoId,
            VideoTitle = report.Video.Title,
            Description = report.Description,
            Status = report.Status,
            CreatedAt = report.CreatedAt,
            ReviewedAt = report.ReviewedAt,
            Reporter = new CommentAuthorDto
            {
                Id = report.ReporterUser.Id,
                Username = report.ReporterUser.Username,
                ProfileImageUrl = report.ReporterUser.Profile?.ProfileImageUrl
            },
            ReviewedBy = report.ReviewedByUser == null ? null : new CommentAuthorDto
            {
                Id = report.ReviewedByUser.Id,
                Username = report.ReviewedByUser.Username,
                ProfileImageUrl = report.ReviewedByUser.Profile?.ProfileImageUrl
            }
        };
    }
}