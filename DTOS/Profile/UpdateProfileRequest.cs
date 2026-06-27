using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Profile;

public class UpdateProfileRequest
{
    [StringLength(2000, ErrorMessage = "Bio cannot exceed 2000 characters.")]
    public string? Bio { get; set; }

    [StringLength(255, ErrorMessage = "ProfileImageUrl cannot exceed 255 characters.")]
    [Url(ErrorMessage = "ProfileImageUrl must be a valid URL.")]
    public string? ProfileImageUrl { get; set; }

    [Phone(ErrorMessage = "Phone must be a valid phone number.")]
    [StringLength(30, ErrorMessage = "Phone cannot exceed 30 characters.")]
    public string? Phone { get; set; }

    [Url(ErrorMessage = "Website must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Website cannot exceed 500 characters.")]
    public string? Website { get; set; }

    [Url(ErrorMessage = "Facebook must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Facebook cannot exceed 500 characters.")]
    public string? Facebook { get; set; }

    [Url(ErrorMessage = "Youtube must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Youtube cannot exceed 500 characters.")]
    public string? Youtube { get; set; }

    [Url(ErrorMessage = "Instagram must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Instagram cannot exceed 500 characters.")]
    public string? Instagram { get; set; }

    [Url(ErrorMessage = "Tiktok must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Tiktok cannot exceed 500 characters.")]
    public string? Tiktok { get; set; }

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string? Address { get; set; }

    [StringLength(200, ErrorMessage = "Headline cannot exceed 200 characters.")]
    public string? Headline { get; set; }

    [StringLength(2000, ErrorMessage = "Experience cannot exceed 2000 characters.")]
    public string? Experience { get; set; }
}
