using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        public UserRole? Role { get; set; }

        [Required]
        public TalentCategory? PrimaryCategory { get; set; }

        [Required]
        public SkillLevel? SkillLevel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProvinceId is required.")]
        public int ProvinceId { get; set; }
    }
}
