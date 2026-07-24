using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Message>> GetConversationAsync(int userId, int otherUserId, int page, int pageSize) =>
            await ConversationQuery(userId, otherUserId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountConversationAsync(int userId, int otherUserId) =>
            await ConversationQuery(userId, otherUserId).CountAsync();

        public async Task MarkConversationAsReadAsync(int userId, int otherUserId) =>
            await _dbSet
                .Where(m => m.ReceiverId == userId && m.SenderId == otherUserId && !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));

        public async Task<int> CountUnreadTotalAsync(int userId) =>
            await _dbSet.CountAsync(m => m.ReceiverId == userId && !m.IsRead);

        // Groups all my messages by "the other participant" and takes the most recent
        // timestamp per group — this is the paginated, ordered list of who I've talked to.
        public async Task<List<int>> GetConversationPartnerIdsPageAsync(int userId, int page, int pageSize) =>
            await _dbSet
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new { PartnerId = g.Key, LastMessageAt = g.Max(m => m.CreatedAt) })
                .OrderByDescending(x => x.LastMessageAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.PartnerId)
                .ToListAsync();

        public async Task<int> CountConversationPartnersAsync(int userId) =>
            await _dbSet
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .CountAsync();

        // Fetches every message between me and the (at most one page's worth of) partner ids
        // in one query, then picks the latest per partner in memory — avoids per-partner
        // round trips and avoids EF Core's flaky translation of "OrderBy().First() per group".
        public async Task<Dictionary<int, Message>> GetLatestMessagesAsync(int userId, IEnumerable<int> partnerIds)
        {
            var ids = partnerIds.ToList();

            var messages = await _dbSet
                .Where(m =>
                    (m.SenderId == userId && ids.Contains(m.ReceiverId)) ||
                    (m.ReceiverId == userId && ids.Contains(m.SenderId)))
                .ToListAsync();

            return messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(m => m.CreatedAt).First());
        }

        public async Task<Dictionary<int, int>> CountUnreadByPartnersAsync(int userId, IEnumerable<int> partnerIds) =>
            await _dbSet
                .Where(m => m.ReceiverId == userId && !m.IsRead && partnerIds.Contains(m.SenderId))
                .GroupBy(m => m.SenderId)
                .Select(g => new { PartnerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PartnerId, x => x.Count);

        public async Task<int> CountAllAsync() =>
            await _dbSet.CountAsync();

        private IQueryable<Message> ConversationQuery(int userId, int otherUserId) =>
            _dbSet.Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId));
    }
}