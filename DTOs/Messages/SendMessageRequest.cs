using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Messages
{
    public class SendMessageRequest
    {
        [Required]
        public int? ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = null!;
    }
}