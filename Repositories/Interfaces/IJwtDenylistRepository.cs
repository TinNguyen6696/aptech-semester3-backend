using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IJwtDenylistRepository : IGenericRepository<JwtDenylist>
    {
        Task<bool> ExistsAsync(string jti);
    }
}
