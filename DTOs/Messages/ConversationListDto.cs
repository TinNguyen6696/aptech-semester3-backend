namespace TalentShowcase.Api.DTOs.Messages
{
    public class ConversationListDto
    {
        public IEnumerable<ConversationDto> Conversations { get; set; } = new List<ConversationDto>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int TotalUnreadCount { get; set; }
    }
}