namespace TalentShowcase.Api.DTOs.Ratings
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}