using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(int userId, int otherUserId, int page, int pageSize);
        Task<int> CountConversationAsync(int userId, int otherUserId);
        Task MarkConversationAsReadAsync(int userId, int otherUserId);
        Task<int> CountUnreadTotalAsync(int userId);
        Task<List<int>> GetConversationPartnerIdsPageAsync(int userId, int page, int pageSize);
        Task<int> CountConversationPartnersAsync(int userId);
        Task<Dictionary<int, Message>> GetLatestMessagesAsync(int userId, IEnumerable<int> partnerIds);
        Task<Dictionary<int, int>> CountUnreadByPartnersAsync(int userId, IEnumerable<int> partnerIds);
        Task<int> CountAllAsync();
    }
}