using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Auth;

public class LoginRequest
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
