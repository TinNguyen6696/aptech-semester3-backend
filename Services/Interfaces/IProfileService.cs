using TaLentShowcase.API.DTOS.Profile;

namespace TaLentShowcase.API.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileResponse> GetProfileAsync(int userId);

    Task UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request);

    Task<UserTalentDto> AddUserTalentAsync(int userId, AddUserTalentRequest request);

    Task UpdateUserTalentAsync(int userId, int userTalentId, UpdateUserTalentRequest request);

    Task DeleteUserTalentAsync(int userId, int userTalentId);

    Task<AchievementDto> AddAchievementAsync(int userId, UpsertAchievementRequest request);

    Task UpdateAchievementAsync(int userId, int achievementId, UpsertAchievementRequest request);

    Task DeleteAchievementAsync(int userId, int achievementId);

    Task<AwardDto> AddAwardAsync(int userId, UpsertAwardRequest request);

    Task UpdateAwardAsync(int userId, int awardId, UpsertAwardRequest request);

    Task DeleteAwardAsync(int userId, int awardId);

    Task<CertificationDto> AddCertificationAsync(int userId, UpsertCertificationRequest request);

    Task UpdateCertificationAsync(int userId, int certificationId, UpsertCertificationRequest request);

    Task DeleteCertificationAsync(int userId, int certificationId);
}
