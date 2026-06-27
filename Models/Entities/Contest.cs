using TaLentShowcase.API.Models.Enums;

namespace TaLentShowcase.API.Models.Entities;

public class Contest : BaseEntity
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int TalentId { get; set; }

    public Talent Talent { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ICollection<ContestEntry> Entries { get; set; } = new List<ContestEntry>();
}