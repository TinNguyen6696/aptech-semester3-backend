using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Users
{
    public class PublicProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public TalentCategory PrimaryCategory { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
        public IEnumerable<AchievementDto> Achievements { get; set; } = new List<AchievementDto>();
    }
}
