using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Community : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public TalentCategory Category { get; set; }
        public int CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; } = null!;
    }
}