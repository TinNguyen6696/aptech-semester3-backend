namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class OpportunityApplicationDto
    {
        public int Id { get; set; }
        public int OpportunityId { get; set; }
        public DateTime AppliedAt { get; set; }
        public OpportunityApplicantDto Applicant { get; set; } = null!;
    }
}
