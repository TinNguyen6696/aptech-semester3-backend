namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class OpportunityListDto
    {
        public IEnumerable<OpportunityDto> Opportunities { get; set; } = new List<OpportunityDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}