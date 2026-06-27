using TaLentShowcase.API.DTOS.Auth;

namespace TaLentShowcase.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request);
    Task LogoutAsync(int userId, string jti, DateTime accessTokenExpiresAt, string refreshToken);
}
