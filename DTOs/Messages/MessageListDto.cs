namespace TalentShowcase.Api.DTOs.Messages
{
    public class MessageListDto
    {
        public IEnumerable<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}