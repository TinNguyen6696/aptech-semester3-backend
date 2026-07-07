using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Auth
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
