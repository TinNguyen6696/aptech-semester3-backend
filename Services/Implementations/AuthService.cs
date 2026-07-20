using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Auth;
using TalentShowcase.Api.Helpers;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IJwtDenylistRepository _jwtDenylistRepo;
        private readonly IGenericRepository<Province> _provinceRepo;
        private readonly IUserTokenRepository _userTokenRepo;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IJwtDenylistRepository jwtDenylistRepo,
            IGenericRepository<Province> provinceRepo,
            IUserTokenRepository userTokenRepo,
            IEmailService emailService,
            AppSettings appSettings,
            JwtHelper jwtHelper)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _jwtDenylistRepo = jwtDenylistRepo;
            _provinceRepo = provinceRepo;
            _userTokenRepo = userTokenRepo;
            _emailService = emailService;
            _appSettings = appSettings;
            _jwtHelper = jwtHelper;
        }

        public async Task<Result<object>> RegisterAsync(RegisterRequest request)
        {
            if (request.Role is not (UserRole.Member or UserRole.Mentor or UserRole.Recruiter))
                return new Result<object> { IsSuccess = false, Message = "Invalid role. Must be Member, Mentor, or Recruiter.", StatusCode = 400 };

            if (!Enum.IsDefined(request.PrimaryCategory!.Value))
                return new Result<object> { IsSuccess = false, Message = "Invalid primary category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.SkillLevel!.Value))
                return new Result<object> { IsSuccess = false, Message = "Invalid skill level.", StatusCode = 400 };

            if (await _userRepo.ExistsByUsernameAsync(request.Username))
                return new Result<object> { IsSuccess = false, Message = "Username already taken.", StatusCode = 400 };

            if (await _userRepo.ExistsByEmailAsync(request.Email))
                return new Result<object> { IsSuccess = false, Message = "Email already in use.", StatusCode = 400 };

            if (await _provinceRepo.GetByIdAsync(request.ProvinceId) == null)
                return new Result<object> { IsSuccess = false, Message = "Invalid province.", StatusCode = 400 };

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHelper.Hash(request.Password),
                Role = request.Role!.Value,
                Profile = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PrimaryCategory = request.PrimaryCategory!.Value,
                    SkillLevel = request.SkillLevel!.Value,
                    ProvinceId = request.ProvinceId
                }
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            await IssueEmailVerificationTokenAsync(user);

            return new Result<object>
            {
                IsSuccess = true,
                Message = "Registration successful. Please check your email to verify your account before logging in.",
                StatusCode = 200
            };
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null || !PasswordHelper.Verify(request.Password, user.PasswordHash))
                return new Result<AuthResponse> { IsSuccess = false, Message = "Invalid email or password.", StatusCode = 401 };

            if (!user.IsActive)
                return new Result<AuthResponse> { IsSuccess = false, Message = "Your account has been deactivated.", StatusCode = 403 };

            if (!user.EmailConfirmed)
                return new Result<AuthResponse> { IsSuccess = false, Message = "Please verify your email before logging in.", StatusCode = 403 };

            var fullUser = await _userRepo.GetByIdWithProfileAsync(user.Id);
            return await IssueTokensAsync(fullUser!, "Login successful.");
        }

        public async Task<Result<object>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var tokenHash = TokenHelper.Hash(request.Token);
            var token = await _userTokenRepo.GetActiveByHashAsync(tokenHash, UserTokenType.EmailVerification);

            if (token == null)
                return new Result<object> { IsSuccess = false, Message = "Invalid or expired verification token.", StatusCode = 400 };

            var user = await _userRepo.GetByIdAsync(token.UserId);
            if (user == null)
                return new Result<object> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            user.EmailConfirmed = true;
            _userRepo.Update(user);

            token.UsedAt = DateTime.UtcNow;
            _userTokenRepo.Update(token);
            await _userTokenRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Email verified successfully. You can now log in.", StatusCode = 200 };
        }

        public async Task<Result<object>> ResendVerificationAsync(ResendVerificationRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            // Only act on an existing, still-unverified account; always return the
            // same message so we don't reveal which emails are registered.
            if (user != null && !user.EmailConfirmed)
            {
                await InvalidateActiveTokensAsync(user.Id, UserTokenType.EmailVerification);
                await IssueEmailVerificationTokenAsync(user);
            }

            return new Result<object> { IsSuccess = true, Message = "If an unverified account exists for that email, a verification link has been sent.", StatusCode = 200 };
        }

        public async Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            // Always return 200 with the same message, even if the email doesn't
            // exist, to avoid leaking which emails are registered.
            if (user != null)
            {
                await InvalidateActiveTokensAsync(user.Id, UserTokenType.PasswordReset);

                var rawToken = TokenHelper.GenerateRawToken();
                var token = new UserToken
                {
                    UserId = user.Id,
                    TokenType = UserTokenType.PasswordReset,
                    TokenHash = TokenHelper.Hash(rawToken),
                    ExpiresAt = DateTime.UtcNow.AddHours(_appSettings.PasswordResetTokenHours)
                };
                await _userTokenRepo.AddAsync(token);
                await _userTokenRepo.SaveChangesAsync();

                var resetUrl = $"{_appSettings.FrontendBaseUrl}/reset-password?token={rawToken}";
                await _emailService.SendPasswordResetAsync(user.Email, user.Username, resetUrl);
            }

            return new Result<object> { IsSuccess = true, Message = "If an account with that email exists, a password reset link has been sent.", StatusCode = 200 };
        }

        public async Task<Result<object>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenHash = TokenHelper.Hash(request.Token);
            var token = await _userTokenRepo.GetActiveByHashAsync(tokenHash, UserTokenType.PasswordReset);

            if (token == null)
                return new Result<object> { IsSuccess = false, Message = "Invalid or expired reset token.", StatusCode = 400 };

            var user = await _userRepo.GetByIdAsync(token.UserId);
            if (user == null)
                return new Result<object> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            user.PasswordHash = PasswordHelper.Hash(request.NewPassword);
            _userRepo.Update(user);

            token.UsedAt = DateTime.UtcNow;
            _userTokenRepo.Update(token);

            // Revoke every active session, same as change-password.
            var refreshTokens = await _refreshTokenRepo.GetActiveByUserIdAsync(user.Id);
            foreach (var rt in refreshTokens)
            {
                rt.Revoked = true;
                _refreshTokenRepo.Update(rt);
            }

            await _userTokenRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Password reset successfully. Please log in.", StatusCode = 200 };
        }

        private async Task IssueEmailVerificationTokenAsync(User user)
        {
            var rawToken = TokenHelper.GenerateRawToken();
            var token = new UserToken
            {
                UserId = user.Id,
                TokenType = UserTokenType.EmailVerification,
                TokenHash = TokenHelper.Hash(rawToken),
                ExpiresAt = DateTime.UtcNow.AddHours(_appSettings.EmailVerificationTokenHours)
            };
            await _userTokenRepo.AddAsync(token);
            await _userTokenRepo.SaveChangesAsync();

            var verifyUrl = $"{_appSettings.FrontendBaseUrl}/verify-email?token={rawToken}";
            await _emailService.SendEmailVerificationAsync(user.Email, user.Username, verifyUrl);
        }

        private async Task InvalidateActiveTokensAsync(int userId, UserTokenType type)
        {
            var existing = await _userTokenRepo.GetActiveByUserIdAsync(userId, type);
            foreach (var t in existing)
            {
                t.UsedAt = DateTime.UtcNow;
                _userTokenRepo.Update(t);
            }
            await _userTokenRepo.SaveChangesAsync();
        }

        public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken);

            if (token == null || token.Revoked || token.ExpiredAt < DateTime.UtcNow)
                return new Result<AuthResponse> { IsSuccess = false, Message = "Invalid or expired refresh token.", StatusCode = 401 };

            token.Revoked = true;
            _refreshTokenRepo.Update(token);
            await _refreshTokenRepo.SaveChangesAsync();

            var fullUser = await _userRepo.GetByIdWithProfileAsync(token.UserId);
            return await IssueTokensAsync(fullUser!, "Token refreshed successfully.");
        }

        public async Task<Result<object>> LogoutAsync(int userId, string jti, DateTime jtiExpiresAt)
        {
            var denyEntry = new JwtDenylist { Jti = jti, ExpiredAt = jtiExpiresAt };
            await _jwtDenylistRepo.AddAsync(denyEntry);

            var tokens = await _refreshTokenRepo.GetActiveByUserIdAsync(userId);
            foreach (var token in tokens)
            {
                token.Revoked = true;
                _refreshTokenRepo.Update(token);
            }

            await _jwtDenylistRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Logged out successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return new Result<object> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            if (!PasswordHelper.Verify(request.CurrentPassword, user.PasswordHash))
                return new Result<object> { IsSuccess = false, Message = "Current password is incorrect.", StatusCode = 400 };

            user.PasswordHash = PasswordHelper.Hash(request.NewPassword);
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            var tokens = await _refreshTokenRepo.GetActiveByUserIdAsync(userId);
            foreach (var token in tokens)
            {
                token.Revoked = true;
                _refreshTokenRepo.Update(token);
            }
            await _refreshTokenRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Password changed successfully. Please log in again.", StatusCode = 200 };
        }

        private async Task<Result<AuthResponse>> IssueTokensAsync(User user, string message)
        {
            var (accessToken, jti, expiresAt) = _jwtHelper.GenerateAccessToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiredAt = _jwtHelper.GetRefreshTokenExpiry()
            };

            await _refreshTokenRepo.AddAsync(refreshTokenEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            var fullUser = user.Profile?.Province != null ? user : await _userRepo.GetByIdWithProfileAsync(user.Id);

            return new Result<AuthResponse>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = message,
                Data = new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = UserMapper.ToDto(fullUser!)
                }
            };
        }
    }
}