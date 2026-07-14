using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Messages;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Result<MessageDto>> SendMessageAsync(int senderId, SendMessageRequest request);
        Task<Result<MessageListDto>> GetConversationAsync(int userId, int otherUserId, int page, int pageSize);
        Task<Result<ConversationListDto>> GetConversationsAsync(int userId, int page, int pageSize);
        Task<Result<object>> MarkConversationAsReadAsync(int userId, int otherUserId);
        Task<Result<int>> GetUnreadCountAsync(int userId);
    }
}