using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class OpportunityApplicantDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public TalentCategory? PrimaryCategory { get; set; }
        public SkillLevel? SkillLevel { get; set; }
    }
}
