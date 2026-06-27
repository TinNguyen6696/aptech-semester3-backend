using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "Username contains invalid characters.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "ConfirmPassword does not match Password.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string LastName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ProvinceId { get; set; }
}
