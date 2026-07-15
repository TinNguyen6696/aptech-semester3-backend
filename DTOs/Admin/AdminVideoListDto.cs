namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminVideoListDto
    {
        public IEnumerable<AdminVideoDto> Videos { get; set; } = new List<AdminVideoDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}