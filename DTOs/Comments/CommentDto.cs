namespace TalentShowcase.Api.DTOs.Comments
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public int LikeCount { get; set; }
        public bool? IsLiked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CommentAuthorDto Author { get; set; } = null!;
    }
}