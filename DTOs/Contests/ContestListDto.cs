namespace TalentShowcase.Api.DTOs.Contests
{
    public class ContestListDto
    {
        public IEnumerable<ContestDto> Contests { get; set; } = new List<ContestDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}