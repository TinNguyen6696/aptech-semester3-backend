namespace TaLentShowcase.API.DTOS.Profile;

public class ProfileResponse
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string Province { get; set; } = string.Empty;

    public UserProfileDto Profile { get; set; } = new();

    public List<UserTalentDto> Talents { get; set; } = [];

    public List<AchievementDto> Achievements { get; set; } = [];

    public List<AwardDto> Awards { get; set; } = [];

    public List<CertificationDto> Certifications { get; set; } = [];
}
