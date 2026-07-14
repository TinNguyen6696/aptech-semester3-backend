namespace TalentShowcase.Api.Models.Entities
{
    public class CommunityPost : BaseEntity
    {
        public int CommunityId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;

        public Community Community { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}