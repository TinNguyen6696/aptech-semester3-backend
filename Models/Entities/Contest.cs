using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Contest : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TalentCategory Category { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User CreatedByUser { get; set; } = null!;
    }
}