using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IUserTokenRepository : IGenericRepository<UserToken>
    {
        Task<UserToken?> GetActiveByHashAsync(string tokenHash, UserTokenType type);
        Task<IEnumerable<UserToken>> GetActiveByUserIdAsync(int userId, UserTokenType type);
    }
}
