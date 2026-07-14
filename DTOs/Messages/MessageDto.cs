namespace TalentShowcase.Api.DTOs.Messages
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }
        public bool IsMine { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}