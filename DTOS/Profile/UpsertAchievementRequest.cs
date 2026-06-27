namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertAchievementRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? AchievementDate { get; set; }
}
