using System.Xml.Linq;
using TaLentShowcase.API.Models.Enums;
using TaLentShowcase.API.Models.JWT;

namespace TaLentShowcase.API.Models.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public int ProvinceId { get; set; }

    public Province Province { get; set; } = null!;

    public UserRole Role { get; set; }

    public ICollection<Video> Videos { get; set; } = new List<Video>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public UserProfile? UserProfile { get; set; }

    public ICollection<Achievement> Achievements { get; set; }
        = new List<Achievement>();

    public ICollection<Award> Awards { get; set; }
        = new List<Award>();

    public ICollection<Certification> Certifications { get; set; }
        = new List<Certification>();

    public ICollection<UserTalent> UserTalents { get; set; }
        = new List<UserTalent>();
}
