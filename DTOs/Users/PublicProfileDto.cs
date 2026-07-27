using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Users
{
    public class PublicProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;

        // Contact details: only populated for signed-in viewers, null for anonymous ones —
        // this endpoint is [AllowAnonymous] over sequential int ids, so returning them
        // unconditionally would let a bot walk /api/users/1..N and harvest every address.
        // PhoneNumber is also null when the user simply never filled it in, so use Email
        // as the "am I allowed to see contact info" flag.
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public UserRole Role { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public TalentCategory PrimaryCategory { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool? IsFollowing { get; set; }
        public IEnumerable<AchievementDto> Achievements { get; set; } = new List<AchievementDto>();
    }
}
