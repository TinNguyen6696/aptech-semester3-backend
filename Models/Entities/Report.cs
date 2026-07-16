using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Report : BaseEntity
    {
        public int VideoId { get; set; }
        public int ReporterUserId { get; set; }
        public string? Description { get; set; }
        public ReportStatus Status { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public Video Video { get; set; } = null!;
        public User ReporterUser { get; set; } = null!;
        public User? ReviewedByUser { get; set; }
    }
}