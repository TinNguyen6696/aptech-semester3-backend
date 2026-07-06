using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Videos
{
    public class PublicVideoDto
    {
        public int Id { get; set; }
        public TalentCategory Category { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public VideoOwnerDto Owner { get; set; } = null!;
    }
}
