using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Communities
{
    public class CreateCommunityPostRequest
    {
        [Required]
        public string Content { get; set; } = null!;
    }
}