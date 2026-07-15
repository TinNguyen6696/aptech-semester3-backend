using TalentShowcase.Api.DTOs.Comments;

namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string ReferenceType { get; set; } = null!;
        public int ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public CommentAuthorDto Author { get; set; } = null!;
    }
}