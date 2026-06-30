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
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IJwtDenylistRepository jwtDenylistRepo,
            JwtHelper jwtHelper)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _jwtDenylistRepo = jwtDenylistRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (request.Role is not (UserRole.Member or UserRole.Mentor or UserRole.Recruiter))
                return new Result<AuthResponse> { IsSuccess = false, Message = "Invalid role. Must be Member, Mentor, or Recruiter.", StatusCode = 400 };

            if (await _userRepo.ExistsByUsernameAsync(request.Username))
                return new Result<AuthResponse> { IsSuccess = false, Message = "Username already taken.", StatusCode = 400 };

            if (await _userRepo.ExistsByEmailAsync(request.Email))
                return new Result<AuthResponse> { IsSuccess = false, Message = "Email already in use.", StatusCode = 400 };

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

            return await IssueTokensAsync(user, "Registration successful.");
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null || !PasswordHelper.Verify(request.Password, user.PasswordHash))
                return new Result<AuthResponse> { IsSuccess = false, Message = "Invalid email or password.", StatusCode = 401 };

            var fullUser = await _userRepo.GetByIdWithProfileAsync(user.Id);
            return await IssueTokensAsync(fullUser!, "Login successful.");
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

        public async Task<Result<UserDto>> MeAsync(int userId)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            if (user == null)
                return new Result<UserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            return new Result<UserDto> { Data = UserMapper.ToDto(user), IsSuccess = true, Message = "User retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> LogoutAsync(int userId, string jti, DateTime jtiExpiresAt)
        {
            var denyEntry = new JwtDenylist { Jti = jti, ExpiredAt = jtiExpiresAt };
            await _jwtDenylistRepo.AddAsync(denyEntry);

            var tokens = await _refreshTokenRepo.GetAllAsync();
            foreach (var token in tokens.Where(t => t.UserId == userId && !t.Revoked))
            {
                token.Revoked = true;
                _refreshTokenRepo.Update(token);
            }

            await _jwtDenylistRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Logged out successfully.", StatusCode = 200 };
        }

        private async Task<Result<AuthResponse>> IssueTokensAsync(User user, string message)
        {
            var (accessToken, jti, expiresAt) = _jwtHelper.GenerateAccessToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiredAt = DateTime.UtcNow.AddDays(7)
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