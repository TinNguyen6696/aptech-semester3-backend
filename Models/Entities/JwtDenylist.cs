namespace TalentShowcase.Api.Models.Entities
{
    public class JwtDenylist : BaseEntity
    {
        public string Jti { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
    }
}
