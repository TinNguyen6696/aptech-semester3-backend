namespace TaLentShowcase.API.Models.Entities;
public class UserProfile : BaseEntity
{
    public int UserId { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? Facebook { get; set; }

    public string? Youtube { get; set; }

    public string? Instagram { get; set; }

    public string? Tiktok { get; set; }

    public string? Address { get; set; }

    public string? Headline { get; set; }

    public string? Experience { get; set; }

    public User User { get; set; } = null!;
}

