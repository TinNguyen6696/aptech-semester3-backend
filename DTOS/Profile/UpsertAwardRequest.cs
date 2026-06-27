using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertAwardRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "AwardName is required.")]
    [StringLength(200, ErrorMessage = "AwardName cannot exceed 200 characters.")]
    public string AwardName { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Organization cannot exceed 200 characters.")]
    public string? Organization { get; set; }

    public DateTime? AwardDate { get; set; }
}
