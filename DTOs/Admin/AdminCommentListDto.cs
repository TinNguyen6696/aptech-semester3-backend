namespace TalentShowcase.Api.DTOs.Admin
{
    public class AdminCommentListDto
    {
        public IEnumerable<AdminCommentDto> Comments { get; set; } = new List<AdminCommentDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}