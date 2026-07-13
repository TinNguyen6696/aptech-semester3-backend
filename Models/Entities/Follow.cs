namespace TalentShowcase.Api.Models.Entities
{
    public class Follow : BaseEntity
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }

        public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
