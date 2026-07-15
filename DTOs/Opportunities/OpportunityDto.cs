using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class OpportunityDto
    {
        public int Id { get; set; }
        public TalentCategory Category { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public CommentAuthorDto PostedBy { get; set; } = null!;
    }
}