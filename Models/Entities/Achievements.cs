namespace TaLentShowcase.API.Models.Entities;

public class Achievement : BaseEntity
{
    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? AchievementDate { get; set; }

    public User User { get; set; } = null!;
}
