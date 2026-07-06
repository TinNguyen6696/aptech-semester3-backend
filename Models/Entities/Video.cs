using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Video : BaseEntity
    {
        public int UserId { get; set; }
        public TalentCategory Category { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }

        public User User { get; set; } = null!;
    }
}
