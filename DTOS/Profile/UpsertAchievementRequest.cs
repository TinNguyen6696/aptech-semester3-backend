using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertAchievementRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; set; }

    public DateTime? AchievementDate { get; set; }
}
