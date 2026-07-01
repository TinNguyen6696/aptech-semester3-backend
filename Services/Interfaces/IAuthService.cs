using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Auth;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> RefreshAsync(string refreshToken);
        Task<Result<object>> LogoutAsync(int userId, string jti, DateTime jtiExpiresAt);
        Task<Result<object>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}