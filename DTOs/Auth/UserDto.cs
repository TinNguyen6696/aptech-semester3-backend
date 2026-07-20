using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public TalentCategory PrimaryCategory { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
    }
}