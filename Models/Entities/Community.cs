using TaLentShowcase.API.Models.Enums;

namespace TaLentShowcase.API.Models.Entities;

public class Community : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int TalentId { get; set; }

    public Talent Talent { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public ICollection<CommunityMember> Members { get; set; } = new List<CommunityMember>();
}
