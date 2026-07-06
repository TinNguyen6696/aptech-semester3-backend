namespace TalentShowcase.Api.DTOs.Videos
{
    public class PublicVideoListDto
    {
        public IEnumerable<PublicVideoDto> Videos { get; set; } = new List<PublicVideoDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
