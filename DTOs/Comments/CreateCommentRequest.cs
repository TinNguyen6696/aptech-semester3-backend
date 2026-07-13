using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Comments
{
    public class CreateCommentRequest
    {
        [Required]
        public string Content { get; set; } = null!;
    }
}