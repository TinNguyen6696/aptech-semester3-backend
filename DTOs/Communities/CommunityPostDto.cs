using TalentShowcase.Api.DTOs.Comments;

namespace TalentShowcase.Api.DTOs.Communities
{
    public class CommunityPostDto
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string Content { get; set; } = null!;
        public int LikeCount { get; set; }
        public bool? IsLiked { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CommentAuthorDto Author { get; set; } = null!;
    }
}