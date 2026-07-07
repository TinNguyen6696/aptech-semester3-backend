using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Auth;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<object>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> RefreshAsync(string refreshToken);
        Task<Result<object>> LogoutAsync(int userId, string jti, DateTime jtiExpiresAt);
        Task<Result<object>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<Result<object>> VerifyEmailAsync(VerifyEmailRequest request);
        Task<Result<object>> ResendVerificationAsync(ResendVerificationRequest request);
        Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<Result<object>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}