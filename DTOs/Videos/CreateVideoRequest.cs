using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Videos
{
    public class CreateVideoRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;

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
