namespace TaLentShowcase.API.Models.Entities;

public class Award : BaseEntity
{
    public int UserId { get; set; }

    public string AwardName { get; set; } = string.Empty;

    public string? Organization { get; set; }

    public DateTime? AwardDate { get; set; }

    public User User { get; set; } = null!;
}