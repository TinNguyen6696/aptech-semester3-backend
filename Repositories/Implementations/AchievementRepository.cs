using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Achievement>> GetByUserIdAsync(int userId) =>
            await _dbSet
                .Where(a => a.UserId == userId)
                .ToListAsync();
    }
}
