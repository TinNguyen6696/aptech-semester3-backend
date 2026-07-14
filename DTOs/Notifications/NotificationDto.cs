namespace TalentShowcase.Api.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}