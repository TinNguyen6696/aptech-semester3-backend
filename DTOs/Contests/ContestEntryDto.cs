using TalentShowcase.Api.DTOs.Comments;

namespace TalentShowcase.Api.DTOs.Contests
{
    public class ContestEntryDto
    {
        public int Id { get; set; }
        public int ContestId { get; set; }
        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public int VoteCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CommentAuthorDto Owner { get; set; } = null!;
    }
}