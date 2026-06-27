namespace TaLentShowcase.API.DTOS.Talent;

public class AddUserTalentRequest
{
    public int TalentId { get; set; }

    public string? Level { get; set; }

    public int? YearsExperience { get; set; }

    public bool IsPrimary { get; set; }
}
