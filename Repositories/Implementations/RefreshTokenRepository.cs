using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await _dbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(int userId) =>
            await _dbSet
                .Where(rt => rt.UserId == userId && !rt.Revoked)
                .ToListAsync();
    }
}
