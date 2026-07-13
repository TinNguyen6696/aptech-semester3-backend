namespace TalentShowcase.Api.Models.Entities
{
    public class Rating : BaseEntity
    {
        public int VideoId { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }

        public Video Video { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}