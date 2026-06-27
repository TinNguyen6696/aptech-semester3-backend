using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Models.JWT;

public class JwtDenylist : BaseEntity
{
    public string Jti { get; set; } = null!;

    public DateTime? ExpiredAt { get; set; }
}
