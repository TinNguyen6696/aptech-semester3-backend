using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Follows
{
    public class FollowUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public TalentCategory? PrimaryCategory { get; set; }
        public SkillLevel? SkillLevel { get; set; }
    }
}
