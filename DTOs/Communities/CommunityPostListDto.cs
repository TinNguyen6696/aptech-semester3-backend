namespace TalentShowcase.Api.DTOs.Communities
{
    public class CommunityPostListDto
    {
        public IEnumerable<CommunityPostDto> Posts { get; set; } = new List<CommunityPostDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}