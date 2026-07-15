namespace TalentShowcase.Api.DTOs.Contests
{
    public class ContestEntryListDto
    {
        public IEnumerable<ContestEntryDto> Entries { get; set; } = new List<ContestEntryDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}