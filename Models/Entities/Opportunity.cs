using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Models.Entities
{
    public class Opportunity : BaseEntity
    {
        public int PostedByUserId { get; set; }
        public TalentCategory Category { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProvinceId { get; set; }

        public User PostedByUser { get; set; } = null!;
        public Province Province { get; set; } = null!;
    }
}
