using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Achievement : BaseEntity
    {
        public int UserId { get; set; }
        public AchievementType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? Issuer { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? Description { get; set; }

        public User User { get; set; } = null!;
    }
}
