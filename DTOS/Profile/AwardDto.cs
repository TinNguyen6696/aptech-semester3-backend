namespace TaLentShowcase.API.DTOS.Profile;

public class AwardDto
{
    public int Id { get; set; }

    public string AwardName { get; set; } = string.Empty;

    public string? Organization { get; set; }

    public DateTime? AwardDate { get; set; }
}
