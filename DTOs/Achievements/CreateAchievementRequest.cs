using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Achievements
{
    public class CreateAchievementRequest
    {
        [Required]
        public AchievementType? Type { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [StringLength(150)]
        public string? Issuer { get; set; }

        public DateTime? IssuedDate { get; set; }

        [StringLength(255)]
        public string? CertificateUrl { get; set; }

        public string? Description { get; set; }
    }
}
