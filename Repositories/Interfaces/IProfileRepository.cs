using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Repositories.Interfaces;

public interface IProfileRepository
{
    Task<User?> GetProfileForReadAsync(int userId);

    Task<User?> GetUserWithProfileAsync(int userId);

    Task<bool> UserExistsAsync(int userId);

    Task<bool> UserTalentExistsAsync(int userId, int talentId);

    Task<Talent?> GetTalentAsync(int talentId);

    Task<UserTalent?> GetUserTalentAsync(int userId, int userTalentId);

    Task<Achievement?> GetAchievementAsync(int userId, int achievementId);

    Task<Award?> GetAwardAsync(int userId, int awardId);

    Task<Certification?> GetCertificationAsync(int userId, int certificationId);

    Task AddUserTalentAsync(UserTalent userTalent);

    Task AddAchievementAsync(Achievement achievement);

    Task AddAwardAsync(Award award);

    Task AddCertificationAsync(Certification certification);

    Task ClearPrimaryUserTalentsAsync(int userId);

    void RemoveUserTalent(UserTalent userTalent);

    void RemoveAchievement(Achievement achievement);

    void RemoveAward(Award award);

    void RemoveCertification(Certification certification);

    Task SaveChangesAsync();
}
