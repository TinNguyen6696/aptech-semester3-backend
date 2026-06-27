namespace TaLentShowcase.API.DTOS.Profile;

public class UserTalentDto
{
    public int Id { get; set; }

    public int TalentId { get; set; }

    public string TalentName { get; set; } = string.Empty;

    public string? Level { get; set; }

    public int? YearsExperience { get; set; }

    public bool IsPrimary { get; set; }
}
