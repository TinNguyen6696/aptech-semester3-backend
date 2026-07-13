namespace TalentShowcase.Api.Models.Entities
{
    public class Comment : BaseEntity
    {
        public int UserId { get; set; }
        public string ReferenceType { get; set; } = null!;
        public int ReferenceId { get; set; }
        public string Content { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}