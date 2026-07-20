using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Reports
{
    public class ReportDto
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Description { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public CommentAuthorDto Reporter { get; set; } = null!;
        public CommentAuthorDto? ReviewedBy { get; set; }
    }
}