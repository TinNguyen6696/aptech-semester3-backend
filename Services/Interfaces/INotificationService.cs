using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Notifications;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(int recipientUserId, string content, string? referenceType, int? referenceId);
        Task<Result<NotificationListDto>> GetMyNotificationsAsync(int userId, int page, int pageSize);
        Task<Result<object>> MarkAsReadAsync(int userId, int notificationId);
        Task<Result<object>> MarkAllAsReadAsync(int userId);
    }
}