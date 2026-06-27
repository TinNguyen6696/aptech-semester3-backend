namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertAwardRequest
{
    public string AwardName { get; set; } = string.Empty;

    public string? Organization { get; set; }

    public DateTime? AwardDate { get; set; }
}
