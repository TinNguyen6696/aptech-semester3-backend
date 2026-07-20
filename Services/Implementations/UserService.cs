using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.DTOs.Auth;
using TalentShowcase.Api.DTOs.Users;
using TalentShowcase.Api.Helpers;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private const int MaxPageSize = 10;

        private readonly IUserRepository _userRepo;
        private readonly IGenericRepository<Province> _provinceRepo;
        private readonly IAchievementRepository _achievementRepo;
        private readonly IFileUploadService _fileUploadService;
        private readonly IFollowRepository _followRepo;

        public UserService(IUserRepository userRepo, IGenericRepository<Province> provinceRepo, IAchievementRepository achievementRepo, IFileUploadService fileUploadService, IFollowRepository followRepo)
        {
            _userRepo = userRepo;
            _provinceRepo = provinceRepo;
            _achievementRepo = achievementRepo;
            _fileUploadService = fileUploadService;
            _followRepo = followRepo;
        }

        public async Task<Result<UserDto>> GetProfileAsync(int userId)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            if (user == null)
                return new Result<UserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            return new Result<UserDto> { Data = UserMapper.ToDto(user), IsSuccess = true, Message = "Profile retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicProfileDto>> GetPublicProfileAsync(int userId, int? currentUserId)
        {
            var user = await _userRepo.GetPublicByIdAsync(userId);

            if (user == null)
                return new Result<PublicProfileDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            var achievements = await _achievementRepo.GetByUserIdAsync(userId);
            var followerCount = await _followRepo.CountFollowersAsync(userId);
            var followingCount = await _followRepo.CountFollowingAsync(userId);

            // null = anonymous viewer or viewing your own profile (no follow button to show either way).
            bool? isFollowing = null;
            if (currentUserId.HasValue && currentUserId.Value != userId)
                isFollowing = await _followRepo.GetAsync(currentUserId.Value, userId) != null;

            var dto = new PublicProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile.LastName,
                Bio = user.Profile.Bio,
                ProfileImageUrl = user.Profile.ProfileImageUrl,
                PrimaryCategory = user.Profile.PrimaryCategory,
                SkillLevel = user.Profile.SkillLevel,
                ProvinceId = user.Profile.ProvinceId,
                ProvinceName = user.Profile.Province!.Name,
                FollowerCount = followerCount,
                FollowingCount = followingCount,
                IsFollowing = isFollowing,
                Achievements = achievements.OrderByDescending(a => a.CreatedAt).Select(ToDto)
            };

            return new Result<PublicProfileDto> { Data = dto, IsSuccess = true, Message = "Profile retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<MentorListDto>> GetMentorsAsync(TalentCategory? category, SkillLevel? skillLevel, int? provinceId, string? search, int page, int pageSize, int? currentUserId)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<MentorListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (skillLevel.HasValue && !Enum.IsDefined(skillLevel.Value))
                return new Result<MentorListDto> { IsSuccess = false, Message = "Invalid skill level.", StatusCode = 400 };

            if (page < 1)
                return new Result<MentorListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<MentorListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _userRepo.CountActiveByRoleAsync(UserRole.Mentor, category, skillLevel, provinceId, search);
            var mentors = (await _userRepo.GetActiveByRolePagedAsync(UserRole.Mentor, category, skillLevel, provinceId, search, page, pageSize)).ToList();
            var followerCounts = await _followRepo.CountFollowersBatchAsync(mentors.Select(m => m.Id));

            // null = anonymous viewer (no follow button to show either way).
            var followingIds = currentUserId.HasValue
                ? await _followRepo.GetFollowingIdsAsync(currentUserId.Value, mentors.Select(m => m.Id))
                : null;

            var result = new MentorListDto
            {
                Mentors = mentors.Select(m => new MentorSummaryDto
                {
                    Id = m.Id,
                    Username = m.Username,
                    FirstName = m.Profile!.FirstName,
                    LastName = m.Profile.LastName,
                    Bio = m.Profile.Bio,
                    ProfileImageUrl = m.Profile.ProfileImageUrl,
                    PrimaryCategory = m.Profile.PrimaryCategory,
                    SkillLevel = m.Profile.SkillLevel,
                    ProvinceId = m.Profile.ProvinceId,
                    ProvinceName = m.Profile.Province!.Name,
                    FollowerCount = followerCounts.GetValueOrDefault(m.Id),
                    IsFollowing = followingIds?.Contains(m.Id)
                }),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<MentorListDto> { Data = result, IsSuccess = true, Message = "Mentors retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<int>> GetMemberCountAsync()
        {
            var count = await _userRepo.CountActiveByRoleAsync(UserRole.Member, null, null, null, null);
            return new Result<int> { Data = count, IsSuccess = true, Message = "Member count retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            if (user == null)
                return new Result<UserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.PrimaryCategory!.Value))
                return new Result<UserDto> { IsSuccess = false, Message = "Invalid primary category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.SkillLevel!.Value))
                return new Result<UserDto> { IsSuccess = false, Message = "Invalid skill level.", StatusCode = 400 };

            if (await _userRepo.ExistsByUsernameAsync(request.Username, userId))
                return new Result<UserDto> { IsSuccess = false, Message = "Username already taken.", StatusCode = 400 };

            if (await _provinceRepo.GetByIdAsync(request.ProvinceId) == null)
                return new Result<UserDto> { IsSuccess = false, Message = "Invalid province.", StatusCode = 400 };

            user.Username = request.Username;
            user.Profile!.FirstName = request.FirstName;
            user.Profile.LastName = request.LastName;
            user.Profile.Bio = request.Bio;
            user.Profile.PhoneNumber = request.PhoneNumber;
            user.Profile.PrimaryCategory = request.PrimaryCategory!.Value;
            user.Profile.SkillLevel = request.SkillLevel!.Value;
            user.Profile.ProvinceId = request.ProvinceId;

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            var updated = await _userRepo.GetByIdWithProfileAsync(userId);
            return new Result<UserDto> { Data = UserMapper.ToDto(updated!), IsSuccess = true, Message = "Profile updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<UserDto>> UpdateAvatarAsync(int userId, string? profileImageUrl)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(userId);

            if (user == null)
                return new Result<UserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            user.Profile!.ProfileImageUrl = profileImageUrl;

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return new Result<UserDto> { Data = UserMapper.ToDto(user), IsSuccess = true, Message = "Avatar updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<IEnumerable<AchievementDto>>> GetAchievementsAsync(int userId)
        {
            var achievements = await _achievementRepo.GetByUserIdAsync(userId);
            var dtos = achievements.OrderByDescending(a => a.CreatedAt).Select(ToDto);

            return new Result<IEnumerable<AchievementDto>> { Data = dtos, IsSuccess = true, Message = "Achievements retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<AchievementDto>> AddAchievementAsync(int userId, CreateAchievementRequest request)
        {
            if (!Enum.IsDefined(request.Type!.Value))
                return new Result<AchievementDto> { IsSuccess = false, Message = "Invalid achievement type.", StatusCode = 400 };

            if (request.IssuedDate.HasValue && request.IssuedDate.Value > DateTime.UtcNow)
                return new Result<AchievementDto> { IsSuccess = false, Message = "Issued date cannot be in the future.", StatusCode = 400 };

            string? certificateUrl = null;
            if (request.CertificateImage != null)
            {
                var upload = await _fileUploadService.UploadCertificateAsync(request.CertificateImage);
                if (!upload.IsSuccess)
                    return new Result<AchievementDto> { IsSuccess = false, Message = upload.Message, StatusCode = upload.StatusCode };

                certificateUrl = upload.Data;
            }

            var achievement = new Achievement
            {
                UserId = userId,
                Type = request.Type!.Value,
                Title = request.Title,
                Issuer = request.Issuer,
                IssuedDate = request.IssuedDate,
                CertificateUrl = certificateUrl,
                ExternalUrl = request.ExternalUrl,
                Description = request.Description
            };

            await _achievementRepo.AddAsync(achievement);
            await _achievementRepo.SaveChangesAsync();

            return new Result<AchievementDto> { Data = ToDto(achievement), IsSuccess = true, Message = "Achievement added successfully.", StatusCode = 201 };
        }

        public async Task<Result<AchievementDto>> UpdateAchievementAsync(int userId, int achievementId, UpdateAchievementRequest request)
        {
            var achievement = await _achievementRepo.GetByIdAsync(achievementId);

            if (achievement == null || achievement.UserId != userId)
                return new Result<AchievementDto> { IsSuccess = false, Message = "Achievement not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.Type!.Value))
                return new Result<AchievementDto> { IsSuccess = false, Message = "Invalid achievement type.", StatusCode = 400 };

            if (request.IssuedDate.HasValue && request.IssuedDate.Value > DateTime.UtcNow)
                return new Result<AchievementDto> { IsSuccess = false, Message = "Issued date cannot be in the future.", StatusCode = 400 };

            if (request.CertificateImage != null)
            {
                var upload = await _fileUploadService.UploadCertificateAsync(request.CertificateImage);
                if (!upload.IsSuccess)
                    return new Result<AchievementDto> { IsSuccess = false, Message = upload.Message, StatusCode = upload.StatusCode };

                achievement.CertificateUrl = upload.Data;
            }

            achievement.Type = request.Type!.Value;
            achievement.Title = request.Title;
            achievement.Issuer = request.Issuer;
            achievement.IssuedDate = request.IssuedDate;
            achievement.ExternalUrl = request.ExternalUrl;
            achievement.Description = request.Description;

            _achievementRepo.Update(achievement);
            await _achievementRepo.SaveChangesAsync();

            return new Result<AchievementDto> { Data = ToDto(achievement), IsSuccess = true, Message = "Achievement updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteAchievementAsync(int userId, int achievementId)
        {
            var achievement = await _achievementRepo.GetByIdAsync(achievementId);

            if (achievement == null || achievement.UserId != userId)
                return new Result<object> { IsSuccess = false, Message = "Achievement not found.", StatusCode = 404 };

            _achievementRepo.Remove(achievement);
            await _achievementRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Achievement deleted successfully.", StatusCode = 200 };
        }

        private static AchievementDto ToDto(Achievement achievement) => new AchievementDto
        {
            Id = achievement.Id,
            Type = achievement.Type,
            Title = achievement.Title,
            Issuer = achievement.Issuer,
            IssuedDate = achievement.IssuedDate,
            CertificateUrl = achievement.CertificateUrl,
            ExternalUrl = achievement.ExternalUrl,
            Description = achievement.Description,
            CreatedAt = achievement.CreatedAt
        };
    }
}
