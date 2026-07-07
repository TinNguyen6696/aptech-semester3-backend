using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Auth
{
    public class ResendVerificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
