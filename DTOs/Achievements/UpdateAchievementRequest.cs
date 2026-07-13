using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Achievements
{
    public class UpdateAchievementRequest
    {
        [Required]
        public AchievementType? Type { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [StringLength(150)]
        public string? Issuer { get; set; }

        public DateTime? IssuedDate { get; set; }

        public IFormFile? CertificateImage { get; set; }

        [Url]
        [StringLength(500)]
        public string? ExternalUrl { get; set; }

        public string? Description { get; set; }
    }
}
