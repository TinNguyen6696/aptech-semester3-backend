namespace TalentShowcase.Api.DTOs.Comments
{
    public class CommentListDto
    {
        public IEnumerable<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}