namespace TalentShowcase.Api.DTOs.Messages
{
    public class ConversationDto
    {
        public int PartnerId { get; set; }
        public string PartnerUsername { get; set; } = null!;
        public string? PartnerProfileImageUrl { get; set; }
        public string LastMessage { get; set; } = null!;
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}