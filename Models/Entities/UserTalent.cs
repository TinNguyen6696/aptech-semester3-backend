namespace TaLentShowcase.API.Models.Entities;

public class UserTalent : BaseEntity
{
    public int UserId { get; set; }

    public int TalentId { get; set; }

    public bool IsPrimary { get; set; }

    public int? YearsExperience { get; set; }

    public string? Level { get; set; }

    public User User { get; set; } = null!;

    public Talent Talent { get; set; } = null!;
}
