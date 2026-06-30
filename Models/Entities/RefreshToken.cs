namespace TalentShowcase.Api.Models.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public bool Revoked { get; set; }
        public DateTime ExpiredAt { get; set; }

        public User User { get; set; } = null!;
    }
}
