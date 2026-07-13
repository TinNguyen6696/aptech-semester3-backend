using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class VideoRepository : GenericRepository<Video>, IVideoRepository
    {
        public VideoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Video>> GetByUserIdAsync(int userId) =>
            await _dbSet
                .Where(v => v.UserId == userId)
                .ToListAsync();

        public async Task<int> CountByUserIdAsync(int userId) =>
            await _dbSet.CountAsync(v => v.UserId == userId);

        public async Task<IEnumerable<Video>> GetPublicAsync(TalentCategory? category, int page, int pageSize) =>
            await PublicQuery(category)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicAsync(TalentCategory? category) =>
            await PublicQuery(category).CountAsync();

        public async Task<Video?> GetPublicByIdAsync(int id) =>
            await _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(v => v.Id == id && v.Visibility == VideoVisibility.Public);

        public async Task<IEnumerable<Video>> GetPublicByUserIdAsync(int userId, int page, int pageSize) =>
            await PublicByUserQuery(userId)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicByUserIdAsync(int userId) =>
            await PublicByUserQuery(userId).CountAsync();

        private IQueryable<Video> PublicByUserQuery(int userId) =>
            _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .Where(v => v.UserId == userId && v.Visibility == VideoVisibility.Public);

        private IQueryable<Video> PublicQuery(TalentCategory? category)
        {
            var query = _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .Where(v => v.Visibility == VideoVisibility.Public);

            if (category.HasValue)
                query = query.Where(v => v.Category == category.Value);

            return query;
        }
    }
}
