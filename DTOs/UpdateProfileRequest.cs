using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs
{
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        public string? Bio { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public TalentCategory? PrimaryCategory { get; set; }

        [Required]
        public SkillLevel? SkillLevel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProvinceId is required.")]
        public int ProvinceId { get; set; }

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
    }
}