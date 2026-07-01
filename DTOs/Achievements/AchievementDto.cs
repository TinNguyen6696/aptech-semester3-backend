using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Achievements
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public AchievementType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? Issuer { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
