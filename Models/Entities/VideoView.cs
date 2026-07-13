namespace TalentShowcase.Api.Models.Entities
{
    public class VideoView : BaseEntity
    {
        public int VideoId { get; set; }
        public int? UserId { get; set; }

        public Video Video { get; set; } = null!;
        public User? User { get; set; }
    }
}