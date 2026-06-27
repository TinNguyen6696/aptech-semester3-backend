using TaLentShowcase.API.Models.Enums;

namespace TaLentShowcase.API.Models.Entities;

public class Opportunity : BaseEntity
{
    public int PostedByUserId { get; set; }

    public User PostedByUser { get; set; } = null!;

    public int TalentId { get; set; }

    public Talent Talent { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ProvinceId { get; set; }

    public Province Province { get; set; } = null!;
}
