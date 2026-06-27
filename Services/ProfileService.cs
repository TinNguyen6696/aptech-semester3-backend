using TaLentShowcase.API.DTOS.Profile;
using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Repositories.Interfaces;
using TaLentShowcase.API.Services.Interfaces;

namespace TaLentShowcase.API.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;

    public ProfileService(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileResponse> GetProfileAsync(int userId)
    {
        var user = await _repository.GetProfileForReadAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new ProfileResponse
        {
            UserId = user.Id,

            Username = user.Username,

            Email = user.Email,

            FullName = $"{user.FirstName} {user.LastName}",

            Bio = user.Bio,

            ProfileImageUrl = user.ProfileImageUrl,

            Province = user.Province.Name,

            Profile = new UserProfileDto
            {
                Phone = user.UserProfile?.Phone,
                Website = user.UserProfile?.Website,
                Facebook = user.UserProfile?.Facebook,
                Youtube = user.UserProfile?.Youtube,
                Instagram = user.UserProfile?.Instagram,
                Tiktok = user.UserProfile?.Tiktok,
                Address = user.UserProfile?.Address,
                Headline = user.UserProfile?.Headline,
                Experience = user.UserProfile?.Experience
            },

            Talents = user.UserTalents.Select(x => new UserTalentDto
            {
                Id = x.Id,
                TalentId = x.TalentId,
                TalentName = x.Talent.Name,
                Level = x.Level,
                YearsExperience = x.YearsExperience,
                IsPrimary = x.IsPrimary
            }).ToList(),

            Achievements = user.Achievements.Select(x => new AchievementDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                AchievementDate = x.AchievementDate
            }).ToList(),

            Awards = user.Awards.Select(x => new AwardDto
            {
                Id = x.Id,
                AwardName = x.AwardName,
                Organization = x.Organization,
                AwardDate = x.AwardDate
            }).ToList(),

            Certifications = user.Certifications.Select(x => new CertificationDto
            {
                Id = x.Id,
                Name = x.Name,
                IssuedBy = x.IssuedBy,
                IssueDate = x.IssueDate,
                ExpiredDate = x.ExpiredDate,
                CertificateUrl = x.CertificateUrl
            }).ToList()
        };
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _repository.GetUserWithProfileAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Bio = request.Bio;
        user.ProfileImageUrl = request.ProfileImageUrl;

        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile
            {
                UserId = user.Id
            };
        }

        user.UserProfile.Phone = request.Phone;
        user.UserProfile.Website = request.Website;
        user.UserProfile.Facebook = request.Facebook;
        user.UserProfile.Youtube = request.Youtube;
        user.UserProfile.Instagram = request.Instagram;
        user.UserProfile.Tiktok = request.Tiktok;
        user.UserProfile.Address = request.Address;
        user.UserProfile.Headline = request.Headline;
        user.UserProfile.Experience = request.Experience;

        await _repository.SaveChangesAsync();
    }

    public async Task<UserTalentDto> AddUserTalentAsync(int userId, AddUserTalentRequest request)
    {
        await EnsureUserExistsAsync(userId);

        var talent = await _repository.GetTalentAsync(request.TalentId);

        if (talent == null)
            throw new KeyNotFoundException("Talent not found.");

        if (await _repository.UserTalentExistsAsync(userId, request.TalentId))
            throw new ArgumentException("Talent already exists in this profile.");

        if (request.IsPrimary)
            await _repository.ClearPrimaryUserTalentsAsync(userId);

        var userTalent = new UserTalent
        {
            UserId = userId,
            TalentId = request.TalentId,
            Level = request.Level,
            YearsExperience = request.YearsExperience,
            IsPrimary = request.IsPrimary
        };

        await _repository.AddUserTalentAsync(userTalent);
        await _repository.SaveChangesAsync();

        return new UserTalentDto
        {
            Id = userTalent.Id,
            TalentId = talent.Id,
            TalentName = talent.Name,
            Level = userTalent.Level,
            YearsExperience = userTalent.YearsExperience,
            IsPrimary = userTalent.IsPrimary
        };
    }

    public async Task UpdateUserTalentAsync(int userId, int userTalentId, UpdateUserTalentRequest request)
    {
        var userTalent = await _repository.GetUserTalentAsync(userId, userTalentId);

        if (userTalent == null)
            throw new KeyNotFoundException("User talent not found.");

        if (request.IsPrimary)
            await _repository.ClearPrimaryUserTalentsAsync(userId);

        userTalent.Level = request.Level;
        userTalent.YearsExperience = request.YearsExperience;
        userTalent.IsPrimary = request.IsPrimary;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteUserTalentAsync(int userId, int userTalentId)
    {
        var userTalent = await _repository.GetUserTalentAsync(userId, userTalentId);

        if (userTalent == null)
            throw new KeyNotFoundException("User talent not found.");

        _repository.RemoveUserTalent(userTalent);
        await _repository.SaveChangesAsync();
    }

    public async Task<AchievementDto> AddAchievementAsync(int userId, UpsertAchievementRequest request)
    {
        await EnsureUserExistsAsync(userId);

        var achievement = new Achievement
        {
            UserId = userId,
            Title = RequireText(request.Title, "Title"),
            Description = request.Description,
            AchievementDate = request.AchievementDate
        };

        await _repository.AddAchievementAsync(achievement);
        await _repository.SaveChangesAsync();

        return MapAchievement(achievement);
    }

    public async Task UpdateAchievementAsync(int userId, int achievementId, UpsertAchievementRequest request)
    {
        var achievement = await _repository.GetAchievementAsync(userId, achievementId);

        if (achievement == null)
            throw new KeyNotFoundException("Achievement not found.");

        achievement.Title = RequireText(request.Title, "Title");
        achievement.Description = request.Description;
        achievement.AchievementDate = request.AchievementDate;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAchievementAsync(int userId, int achievementId)
    {
        var achievement = await _repository.GetAchievementAsync(userId, achievementId);

        if (achievement == null)
            throw new KeyNotFoundException("Achievement not found.");

        _repository.RemoveAchievement(achievement);
        await _repository.SaveChangesAsync();
    }

    public async Task<AwardDto> AddAwardAsync(int userId, UpsertAwardRequest request)
    {
        await EnsureUserExistsAsync(userId);

        var award = new Award
        {
            UserId = userId,
            AwardName = RequireText(request.AwardName, "AwardName"),
            Organization = request.Organization,
            AwardDate = request.AwardDate
        };

        await _repository.AddAwardAsync(award);
        await _repository.SaveChangesAsync();

        return MapAward(award);
    }

    public async Task UpdateAwardAsync(int userId, int awardId, UpsertAwardRequest request)
    {
        var award = await _repository.GetAwardAsync(userId, awardId);

        if (award == null)
            throw new KeyNotFoundException("Award not found.");

        award.AwardName = RequireText(request.AwardName, "AwardName");
        award.Organization = request.Organization;
        award.AwardDate = request.AwardDate;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAwardAsync(int userId, int awardId)
    {
        var award = await _repository.GetAwardAsync(userId, awardId);

        if (award == null)
            throw new KeyNotFoundException("Award not found.");

        _repository.RemoveAward(award);
        await _repository.SaveChangesAsync();
    }

    public async Task<CertificationDto> AddCertificationAsync(int userId, UpsertCertificationRequest request)
    {
        await EnsureUserExistsAsync(userId);

        var certification = new Certification
        {
            UserId = userId,
            Name = RequireText(request.Name, "Name"),
            IssuedBy = request.IssuedBy,
            IssueDate = request.IssueDate,
            ExpiredDate = request.ExpiredDate,
            CertificateUrl = request.CertificateUrl
        };

        await _repository.AddCertificationAsync(certification);
        await _repository.SaveChangesAsync();

        return MapCertification(certification);
    }

    public async Task UpdateCertificationAsync(int userId, int certificationId, UpsertCertificationRequest request)
    {
        var certification = await _repository.GetCertificationAsync(userId, certificationId);

        if (certification == null)
            throw new KeyNotFoundException("Certification not found.");

        certification.Name = RequireText(request.Name, "Name");
        certification.IssuedBy = request.IssuedBy;
        certification.IssueDate = request.IssueDate;
        certification.ExpiredDate = request.ExpiredDate;
        certification.CertificateUrl = request.CertificateUrl;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteCertificationAsync(int userId, int certificationId)
    {
        var certification = await _repository.GetCertificationAsync(userId, certificationId);

        if (certification == null)
            throw new KeyNotFoundException("Certification not found.");

        _repository.RemoveCertification(certification);
        await _repository.SaveChangesAsync();
    }

    private async Task EnsureUserExistsAsync(int userId)
    {
        if (!await _repository.UserExistsAsync(userId))
            throw new KeyNotFoundException("User not found.");
    }

    private static string RequireText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.");

        return value.Trim();
    }

    private static AchievementDto MapAchievement(Achievement achievement)
    {
        return new AchievementDto
        {
            Id = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description,
            AchievementDate = achievement.AchievementDate
        };
    }

    private static AwardDto MapAward(Award award)
    {
        return new AwardDto
        {
            Id = award.Id,
            AwardName = award.AwardName,
            Organization = award.Organization,
            AwardDate = award.AwardDate
        };
    }

    private static CertificationDto MapCertification(Certification certification)
    {
        return new CertificationDto
        {
            Id = certification.Id,
            Name = certification.Name,
            IssuedBy = certification.IssuedBy,
            IssueDate = certification.IssueDate,
            ExpiredDate = certification.ExpiredDate,
            CertificateUrl = certification.CertificateUrl
        };
    }
}
