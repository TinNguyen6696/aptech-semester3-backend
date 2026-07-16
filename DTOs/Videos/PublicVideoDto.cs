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
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public bool? IsLiked { get; set; }
        public int CommentCount { get; set; }
        public double? AverageRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public VideoOwnerDto Owner { get; set; } = null!;
    }
}
