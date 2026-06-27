using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaLentShowcase.API.DTOS.Auth;
using TaLentShowcase.API.Infrastructure.Auth;
using TaLentShowcase.API.Middleware;
using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Models.Enums;
using TaLentShowcase.API.Models.JWT;
using TaLentShowcase.API.Repositories.Interfaces;
using TaLentShowcase.API.Services.Interfaces;

namespace TaLentShowcase.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly PasswordService _passwordService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IAuthRepository repository,
        PasswordService passwordService,
        IOptions<JwtSettings> jwtSettings)
    {
        _repository = repository;
        _passwordService = passwordService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _repository.UsernameExistsAsync(username))
        {
            throw new ConflictException("Username already exists.");
        }

        if (await _repository.EmailExistsAsync(email))
        {
            throw new ConflictException("Email already exists.");
        }

        if (!await _repository.ProvinceExistsAsync(request.ProvinceId))
        {
            throw new KeyNotFoundException("Province not found.");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _passwordService.Hash(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            ProvinceId = request.ProvinceId,
            Role = UserRole.Member
        };

        _repository.AddUser(user);
        await _repository.SaveChangesAsync();

        var response = CreateTokens(user);
        await _repository.SaveChangesAsync();
        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
        if (user is null || !_passwordService.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException("Invalid username, email or password.");
        }

        var response = CreateTokens(user);
        await _repository.SaveChangesAsync();
        return response;
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await _repository.GetRefreshTokenAsync(tokenHash);

        if (storedToken is null || storedToken.Revoked ||
            !storedToken.ExpiredAt.HasValue || storedToken.ExpiredAt <= DateTime.UtcNow)
        {
            throw new InvalidCredentialsException("Refresh token is invalid or expired.");
        }

        storedToken.Revoked = true;
        var response = CreateTokens(storedToken.User);
        await _repository.SaveChangesAsync();
        return response;
    }

    public async Task LogoutAsync(
        int userId,
        string jti,
        DateTime accessTokenExpiresAt,
        string refreshToken)
    {
        var storedToken = await _repository.GetRefreshTokenAsync(HashToken(refreshToken));
        if (storedToken is not null && storedToken.UserId == userId)
        {
            storedToken.Revoked = true;
        }

        if (!await _repository.IsJtiDeniedAsync(jti))
        {
            _repository.AddDeniedToken(new JwtDenylist
            {
                Jti = jti,
                ExpiredAt = accessTokenExpiresAt
            });
        }

        await _repository.SaveChangesAsync();
    }

    private AuthResponse CreateTokens(User user)
    {
        var now = DateTime.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(_jwtSettings.AccessTokenMinutes);
        var jti = Guid.NewGuid().ToString("N");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            now,
            accessTokenExpiresAt,
            credentials);

        var rawRefreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiresAt = now.AddDays(_jwtSettings.RefreshTokenDays);
        _repository.AddRefreshToken(new RefreshToken
        {
            Token = HashToken(rawRefreshToken),
            User = user,
            Revoked = false,
            ExpiredAt = refreshTokenExpiresAt
        });

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
