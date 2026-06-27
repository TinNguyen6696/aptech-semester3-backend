namespace TaLentShowcase.API.Models.Entities;

public class ContestEntry : BaseEntity
{
    public int ContestId { get; set; }

    public Contest Contest { get; set; } = null!;

    public int VideoId { get; set; }

    public Video Video { get; set; } = null!;

    public ICollection<ContestVote> Votes { get; set; } = new List<ContestVote>();
}
