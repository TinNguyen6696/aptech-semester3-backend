namespace TalentShowcase.Api.Models.Entities
{
    public class ContestVote : BaseEntity
    {
        public int ContestEntryId { get; set; }
        public int UserId { get; set; }

        public ContestEntry ContestEntry { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}