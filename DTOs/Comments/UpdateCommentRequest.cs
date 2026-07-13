using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Comments
{
    public class UpdateCommentRequest
    {
        [Required]
        public string Content { get; set; } = null!;
    }
}