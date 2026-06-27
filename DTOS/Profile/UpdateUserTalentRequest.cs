namespace TaLentShowcase.API.DTOS.Profile;

public class UpdateUserTalentRequest
{
    public string? Level { get; set; }

    public int? YearsExperience { get; set; }

    public bool IsPrimary { get; set; }
}
