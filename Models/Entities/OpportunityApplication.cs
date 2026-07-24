namespace TalentShowcase.Api.Models.Entities
{
    public class OpportunityApplication : BaseEntity
    {
        public int OpportunityId { get; set; }
        public int ApplicantUserId { get; set; }

        public Opportunity Opportunity { get; set; } = null!;
        public User ApplicantUser { get; set; } = null!;
    }
}
