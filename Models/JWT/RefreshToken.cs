using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Models.JWT;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public bool Revoked { get; set; }

    public DateTime? ExpiredAt { get; set; }
}
