using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _notificationService.GetMyNotificationsAsync(CurrentUserId, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _notificationService.MarkAllAsReadAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }
    }
}