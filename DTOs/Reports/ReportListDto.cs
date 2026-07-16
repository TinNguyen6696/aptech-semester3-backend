namespace TalentShowcase.Api.DTOs.Reports
{
    public class ReportListDto
    {
        public IEnumerable<ReportDto> Reports { get; set; } = new List<ReportDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}