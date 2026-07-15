using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminVideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public TalentCategory Category { get; set; }
        public VideoVisibility Visibility { get; set; }
        public DateTime CreatedAt { get; set; }
        public CommentAuthorDto Owner { get; set; } = null!;
    }
}