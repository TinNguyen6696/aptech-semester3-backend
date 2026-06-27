namespace TaLentShowcase.API.DTOS.Profile;

public class AchievementDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? AchievementDate { get; set; }
}
