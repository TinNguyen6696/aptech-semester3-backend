using TalentShowcase.Api.Models.Entities;

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
    }
}
