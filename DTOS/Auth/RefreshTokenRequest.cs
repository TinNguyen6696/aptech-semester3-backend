using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
