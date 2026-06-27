namespace TaLentShowcase.API.Models.Entities;

public class Follow : BaseEntity
{
    public int FollowerId { get; set; }

    public User Follower { get; set; } = null!;

    public int FollowingId { get; set; }

    public User Following { get; set; } = null!;
}
