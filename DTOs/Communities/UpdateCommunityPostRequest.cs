using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Communities
{
    public class UpdateCommunityPostRequest
    {
        [Required]
        public string Content { get; set; } = null!;
    }
}