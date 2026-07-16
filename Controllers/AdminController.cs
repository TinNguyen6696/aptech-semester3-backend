using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Reports;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;
        private readonly IVideoService _videoService;

        public AdminController(IAdminService adminService, IReportService reportService, IVideoService videoService)
        {
            _adminService = adminService;
            _reportService = reportService;
            _videoService = videoService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserRole? role, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _adminService.GetUsersAsync(role, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("users/{id:int}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var result = await _adminService.ToggleUserActiveAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("videos")]
        public async Task<IActionResult> GetVideos([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _adminService.GetVideosAsync(page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("videos/{id:int}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var result = await _videoService.DeleteVideoAsync(CurrentUserId, id, isAdmin: true);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _adminService.GetCommentsAsync(page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports([FromQuery] ReportStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _reportService.GetReportsAsync(status, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("reports/{id:int}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var result = await _reportService.GetReportByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("reports/{id:int}/status")]
        public async Task<IActionResult> UpdateReportStatus(int id, [FromBody] UpdateReportStatusRequest request)
        {
            var result = await _reportService.UpdateReportStatusAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}