namespace TalentShowcase.Api.DTOs.Follows
{
    public class FollowListDto
    {
        public IEnumerable<FollowUserDto> Users { get; set; } = new List<FollowUserDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
