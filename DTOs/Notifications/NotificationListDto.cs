namespace TalentShowcase.Api.DTOs.Notifications
{
    public class NotificationListDto
    {
        public IEnumerable<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int UnreadCount { get; set; }
    }
}