using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class UserToken : BaseEntity
    {
        public int UserId { get; set; }
        public UserTokenType TokenType { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
