namespace TalentShowcase.Api.Models.Entities
{
    public class ContestEntry : BaseEntity
    {
        public int ContestId { get; set; }
        public int VideoId { get; set; }

        public Contest Contest { get; set; } = null!;
        public Video Video { get; set; } = null!;
    }
}