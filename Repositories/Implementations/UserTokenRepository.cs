using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class UserTokenRepository : GenericRepository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(AppDbContext context) : base(context) { }

        public async Task<UserToken?> GetActiveByHashAsync(string tokenHash, UserTokenType type) =>
            await _dbSet.FirstOrDefaultAsync(t =>
                t.TokenHash == tokenHash &&
                t.TokenType == type &&
                t.UsedAt == null &&
                t.ExpiresAt > DateTime.UtcNow);

        public async Task<IEnumerable<UserToken>> GetActiveByUserIdAsync(int userId, UserTokenType type) =>
            await _dbSet
                .Where(t =>
                    t.UserId == userId &&
                    t.TokenType == type &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
    }
}
