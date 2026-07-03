using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs
{
    public class UpdateAvatarRequest
    {
        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
    }
}
