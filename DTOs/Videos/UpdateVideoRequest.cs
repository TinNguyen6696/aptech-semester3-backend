using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Videos
{
    public class UpdateVideoRequest
    {
        [Required]
        public TalentCategory? Category { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public VideoVisibility? Visibility { get; set; }
    }
}
