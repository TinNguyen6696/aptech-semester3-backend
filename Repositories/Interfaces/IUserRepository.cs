using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username, int excludeUserId);
        Task<User?> GetByIdWithProfileAsync(int id);
        Task<User?> GetPublicByIdAsync(int id);
        Task<Dictionary<int, User>> GetByIdsWithProfileAsync(IEnumerable<int> ids);
        Task<IEnumerable<User>> GetAllPagedAsync(UserRole? role, int page, int pageSize);
        Task<int> CountAllAsync(UserRole? role);
        Task<Dictionary<UserRole, int>> CountByRoleAsync();
        Task<int> CountCreatedSinceAsync(DateTime since);
    }
}
