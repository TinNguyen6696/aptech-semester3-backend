using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Data;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;

namespace TalentShowcase.Api.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> ExistsByEmailAsync(string email) =>
            await _dbSet.AnyAsync(u => u.Email == email);

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _dbSet.AnyAsync(u => u.Username == username);

        public async Task<bool> ExistsByUsernameAsync(string username, int excludeUserId) =>
            await _dbSet.AnyAsync(u => u.Username == username && u.Id != excludeUserId);

        public async Task<User?> GetByIdWithProfileAsync(int id) =>
            await _dbSet
                .Include(u => u.Profile)
                .ThenInclude(p => p!.Province)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetPublicByIdAsync(int id) =>
            await _dbSet
                .Include(u => u.Profile)
                .ThenInclude(p => p!.Province)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        public async Task<Dictionary<int, User>> GetByIdsWithProfileAsync(IEnumerable<int> ids) =>
            await _dbSet
                .Include(u => u.Profile)
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u);

        public async Task<IEnumerable<User>> GetAllPagedAsync(UserRole? role, int page, int pageSize) =>
            await RoleQuery(role)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountAllAsync(UserRole? role) =>
            await RoleQuery(role).CountAsync();

        public async Task<Dictionary<UserRole, int>> CountByRoleAsync() =>
            await _dbSet
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Role, x => x.Count);

        public async Task<int> CountCreatedSinceAsync(DateTime since) =>
            await _dbSet.CountAsync(u => u.CreatedAt >= since);

        public async Task<int> CountActiveAsync() =>
            await _dbSet.CountAsync(u => u.IsActive);

        public async Task<IEnumerable<User>> GetActiveByRolePagedAsync(UserRole role, TalentCategory? category, SkillLevel? skillLevel, int? provinceId, string? search, int page, int pageSize) =>
            await ActiveRoleQuery(role, category, skillLevel, provinceId, search)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<int> CountActiveByRoleAsync(UserRole role, TalentCategory? category, SkillLevel? skillLevel, int? provinceId, string? search) =>
            await ActiveRoleQuery(role, category, skillLevel, provinceId, search).CountAsync();

        private IQueryable<User> ActiveRoleQuery(UserRole role, TalentCategory? category, SkillLevel? skillLevel, int? provinceId, string? search)
        {
            var query = _dbSet
                .Include(u => u.Profile)
                .ThenInclude(p => p!.Province)
                .Where(u => u.Role == role && u.IsActive);

            if (category.HasValue)
                query = query.Where(u => u.Profile!.PrimaryCategory == category.Value);

            if (skillLevel.HasValue)
                query = query.Where(u => u.Profile!.SkillLevel == skillLevel.Value);

            if (provinceId.HasValue)
                query = query.Where(u => u.Profile!.ProvinceId == provinceId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    u.Username.Contains(search) ||
                    u.Profile!.FirstName.Contains(search) ||
                    u.Profile.LastName.Contains(search));

            return query;
        }

        private IQueryable<User> RoleQuery(UserRole? role)
        {
            var query = _dbSet.Include(u => u.Profile).AsQueryable();

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            return query;
        }
    }
}
