namespace TaLentShowcase.API.Models.Entities;

public class CommunityMember : BaseEntity
{
    public int CommunityId { get; set; }

    public Community Community { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}