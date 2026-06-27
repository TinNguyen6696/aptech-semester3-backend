namespace TaLentShowcase.API.Models.Entities;

public class ContestVote : BaseEntity
{
    public int ContestEntryId { get; set; }

    public ContestEntry ContestEntry { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
