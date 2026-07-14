using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Notifications;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private const int MaxPageSize = 10;

        private readonly INotificationRepository _notificationRepo;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepo, ILogger<NotificationService> logger)
        {
            _notificationRepo = notificationRepo;
            _logger = logger;
        }

        public async Task CreateAsync(int recipientUserId, string content, string? referenceType, int? referenceId)
        {
            try
            {
                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = recipientUserId,
                    Content = content,
                    ReferenceType = referenceType,
                    ReferenceId = referenceId,
                    IsRead = false
                });
                await _notificationRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Best-effort: the action that triggered this (a like/follow/comment) already
                // succeeded by the time this runs, so a notification write failure must never
                // bubble up and turn that into a failed request.
                _logger.LogWarning(ex, "Failed to create notification for user {UserId}", recipientUserId);
            }
        }

        public async Task<Result<NotificationListDto>> GetMyNotificationsAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<NotificationListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<NotificationListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _notificationRepo.CountByUserIdAsync(userId);
            var unreadCount = await _notificationRepo.CountUnreadByUserIdAsync(userId);
            var notifications = await _notificationRepo.GetByUserIdAsync(userId, page, pageSize);

            var result = new NotificationListDto
            {
                Notifications = notifications.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                UnreadCount = unreadCount
            };

            return new Result<NotificationListDto> { Data = result, IsSuccess = true, Message = "Notifications retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            if (notification == null || notification.UserId != userId)
                return new Result<object> { IsSuccess = false, Message = "Notification not found.", StatusCode = 404 };

            notification.IsRead = true;
            _notificationRepo.Update(notification);
            await _notificationRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Notification marked as read.", StatusCode = 200 };
        }

        public async Task<Result<object>> MarkAllAsReadAsync(int userId)
        {
            await _notificationRepo.MarkAllAsReadAsync(userId);
            return new Result<object> { IsSuccess = true, Message = "All notifications marked as read.", StatusCode = 200 };
        }

        private static NotificationDto ToDto(Notification n) => new NotificationDto
        {
            Id = n.Id,
            Content = n.Content,
            ReferenceType = n.ReferenceType,
            ReferenceId = n.ReferenceId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        };
    }
}