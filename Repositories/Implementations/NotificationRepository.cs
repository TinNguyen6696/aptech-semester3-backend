using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, int page, int pageSize) =>
            await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountByUserIdAsync(int userId) =>
            await _dbSet.CountAsync(n => n.UserId == userId);

        public async Task<int> CountUnreadByUserIdAsync(int userId) =>
            await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkAllAsReadAsync(int userId) =>
            await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }
}