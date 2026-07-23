using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Constants;
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

        public async Task<IEnumerable<Video>> GetPublicAsync(TalentCategory? category, int? provinceId, SkillLevel? skillLevel, VideoSortBy sortBy, int page, int pageSize)
        {
            var query = PublicQuery(category, provinceId, skillLevel);

            query = sortBy switch
            {
                VideoSortBy.MostLiked => query.OrderByDescending(v =>
                    _context.Likes.Count(l => l.ReferenceType == ReferenceTypes.Video && l.ReferenceId == v.Id)),
                _ => query.OrderByDescending(v => v.CreatedAt)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountPublicAsync(TalentCategory? category, int? provinceId, SkillLevel? skillLevel) =>
            await PublicQuery(category, provinceId, skillLevel).CountAsync();

        public async Task<Video?> GetPublicByIdAsync(int id) =>
            await _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(v => v.Id == id && v.Visibility == VideoVisibility.Public && v.User.IsActive && !v.IsRemovedByAdmin);

        public async Task<IEnumerable<Video>> GetPublicByUserIdAsync(int userId, int page, int pageSize) =>
            await PublicByUserQuery(userId)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountPublicByUserIdAsync(int userId) =>
            await PublicByUserQuery(userId).CountAsync();

        // Admin-only: every video regardless of Visibility, for moderation.
        public async Task<IEnumerable<Video>> GetAllPagedAsync(int page, int pageSize) =>
            await _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAllAsync() =>
            await _dbSet.CountAsync();

        public async Task<int> CountCreatedSinceAsync(DateTime since) =>
            await _dbSet.CountAsync(v => v.CreatedAt >= since);

        private IQueryable<Video> PublicByUserQuery(int userId) =>
            _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .Where(v => v.UserId == userId && v.Visibility == VideoVisibility.Public && v.User.IsActive && !v.IsRemovedByAdmin);

        private IQueryable<Video> PublicQuery(TalentCategory? category, int? provinceId, SkillLevel? skillLevel)
        {
            var query = _dbSet
                .Include(v => v.User)
                    .ThenInclude(u => u.Profile)
                .Where(v => v.Visibility == VideoVisibility.Public && v.User.IsActive && !v.IsRemovedByAdmin);

            if (category.HasValue)
                query = query.Where(v => v.Category == category.Value);

            // Filters on the uploader's profile (user_profiles is a required 1:1 on User, so no null risk).
            if (provinceId.HasValue)
                query = query.Where(v => v.User.Profile!.ProvinceId == provinceId.Value);

            if (skillLevel.HasValue)
                query = query.Where(v => v.User.Profile!.SkillLevel == skillLevel.Value);

            return query;
        }
    }
}
