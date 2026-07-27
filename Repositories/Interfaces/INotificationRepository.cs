using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, int page, int pageSize);
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task<int> CountByUserIdAsync(int userId);
        Task<int> CountUnreadByUserIdAsync(int userId);
        Task MarkAllAsReadAsync(int userId);
    }
}