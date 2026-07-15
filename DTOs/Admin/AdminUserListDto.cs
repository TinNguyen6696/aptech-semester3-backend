namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminUserListDto
    {
        public IEnumerable<AdminUserDto> Users { get; set; } = new List<AdminUserDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}