namespace TalentShowcase.Api.DTOs.Users
{
    public class MentorListDto
    {
        public IEnumerable<MentorSummaryDto> Mentors { get; set; } = new List<MentorSummaryDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}