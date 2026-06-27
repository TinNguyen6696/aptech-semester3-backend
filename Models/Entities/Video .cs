using System.Xml.Linq;
using TaLentShowcase.API.Models.Enums;

namespace TaLentShowcase.API.Models.Entities;

public class Video : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int TalentId { get; set; }

    public Talent Talent { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public VideoVisibility Visibility { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public ICollection<VideoView> VideoViews { get; set; } = new List<VideoView>();

    public ICollection<ContestEntry> ContestEntries { get; set; } = new List<ContestEntry>();
}
