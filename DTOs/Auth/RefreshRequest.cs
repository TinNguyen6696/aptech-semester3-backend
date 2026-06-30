using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Auth
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}