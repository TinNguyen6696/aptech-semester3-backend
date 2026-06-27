using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Profile;

public class AddUserTalentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "TalentId must be greater than 0.")]
    public int TalentId { get; set; }

    [StringLength(50, ErrorMessage = "Level cannot exceed 50 characters.")]
    public string? Level { get; set; }

    [Range(0, 100, ErrorMessage = "YearsExperience must be between 0 and 100.")]
    public int? YearsExperience { get; set; }

    public bool IsPrimary { get; set; }
}
