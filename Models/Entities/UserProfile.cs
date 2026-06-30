using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class UserProfile : BaseEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int ProvinceId { get; set; }
        public TalentCategory PrimaryCategory { get; set; }

        public User User { get; set; } = null!;
        public Province Province { get; set; } = null!;
    }
}
