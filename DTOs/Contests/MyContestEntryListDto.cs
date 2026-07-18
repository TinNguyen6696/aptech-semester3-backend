namespace TalentShowcase.Api.DTOs.Contests
{
    public class MyContestEntryListDto
    {
        public IEnumerable<MyContestEntryDto> Entries { get; set; } = new List<MyContestEntryDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
