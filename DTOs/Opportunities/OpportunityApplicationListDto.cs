namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class OpportunityApplicationListDto
    {
        public IEnumerable<OpportunityApplicationDto> Applications { get; set; } = new List<OpportunityApplicationDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
