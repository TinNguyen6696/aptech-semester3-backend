using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.DTOs.Auth;
using TalentShowcase.Api.DTOs.Users;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetProfileAsync(int userId);
        Task<Result<PublicProfileDto>> GetPublicProfileAsync(int userId, int? currentUserId);
        Task<Result<MentorListDto>> GetMentorsAsync(int page, int pageSize);
        Task<Result<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<Result<UserDto>> UpdateAvatarAsync(int userId, string? profileImageUrl);

        Task<Result<IEnumerable<AchievementDto>>> GetAchievementsAsync(int userId);
        Task<Result<AchievementDto>> AddAchievementAsync(int userId, CreateAchievementRequest request);
        Task<Result<AchievementDto>> UpdateAchievementAsync(int userId, int achievementId, UpdateAchievementRequest request);
        Task<Result<object>> DeleteAchievementAsync(int userId, int achievementId);
    }
}
