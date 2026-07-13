namespace TalentShowcase.Api.Models.Entities
{
    public class Like : BaseEntity
    {
        public int UserId { get; set; }
        public string ReferenceType { get; set; } = null!;
        public int ReferenceId { get; set; }

        public User User { get; set; } = null!;
    }
}
