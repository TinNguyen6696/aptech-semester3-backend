using Microsoft.EntityFrameworkCore;
using TaLentShowcase.API.Infrastructure.Persistence;
using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Models.JWT;
using TaLentShowcase.API.Repositories.Interfaces;

namespace TaLentShowcase.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuthRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        var normalized = usernameOrEmail.Trim();
        return _dbContext.Users.FirstOrDefaultAsync(user =>
            user.Username == normalized || user.Email == normalized);
    }

    public Task<bool> UsernameExistsAsync(string username)
    {
        var normalized = username.Trim();
        return _dbContext.Users.AnyAsync(user => user.Username == normalized);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.Trim();
        return _dbContext.Users.AnyAsync(user => user.Email == normalized);
    }

    public Task<bool> ProvinceExistsAsync(int provinceId) =>
        _dbContext.Provinces.AnyAsync(province => province.Id == provinceId);

    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash) =>
        _dbContext.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.Token == tokenHash);

    public Task<bool> IsJtiDeniedAsync(string jti) =>
        _dbContext.JwtDenylists.AnyAsync(token =>
            token.Jti == jti && token.ExpiredAt > DateTime.UtcNow);

    public void AddUser(User user) => _dbContext.Users.Add(user);

    public void AddRefreshToken(RefreshToken refreshToken) =>
        _dbContext.RefreshTokens.Add(refreshToken);

    public void AddDeniedToken(JwtDenylist deniedToken) =>
        _dbContext.JwtDenylists.Add(deniedToken);

    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}
