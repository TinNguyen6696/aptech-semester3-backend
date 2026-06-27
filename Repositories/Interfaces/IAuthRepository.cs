using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Models.JWT;

namespace TaLentShowcase.API.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> ProvinceExistsAsync(int provinceId);
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
    Task<bool> IsJtiDeniedAsync(string jti);
    void AddUser(User user);
    void AddRefreshToken(RefreshToken refreshToken);
    void AddDeniedToken(JwtDenylist deniedToken);
    Task SaveChangesAsync();
}
