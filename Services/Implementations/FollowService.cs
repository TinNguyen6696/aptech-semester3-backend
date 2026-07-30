using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Follows;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class FollowService : IFollowService
    {
        private const int MaxPageSize = 10;

        private readonly IFollowRepository _followRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;

        public FollowService(IFollowRepository followRepo, IUserRepository userRepo, INotificationService notificationService)
        {
            _followRepo = followRepo;
            _userRepo = userRepo;
            _notificationService = notificationService;
        }

        public async Task<Result<object>> ToggleFollowAsync(int followerId, int followingId)
        {
            if (followerId == followingId)
                return new Result<object> { IsSuccess = false, Message = "You cannot follow yourself.", StatusCode = 400 };

            var target = await _userRepo.GetPublicByIdAsync(followingId);
            if (target == null)
                return new Result<object> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            var existing = await _followRepo.GetAsync(followerId, followingId);
            if (existing != null)
            {
                _followRepo.Remove(existing);
                await _followRepo.SaveChangesAsync();
                return new Result<object> { IsSuccess = true, Message = "Unfollowed.", StatusCode = 200 };
            }

            await _followRepo.AddAsync(new Follow { FollowerId = followerId, FollowingId = followingId });
            await _followRepo.SaveChangesAsync();

            var follower = await _userRepo.GetByIdAsync(followerId);
            if (follower != null)
                await _notificationService.CreateAsync(followingId, $"{follower.Username} followed you.", ReferenceTypes.User, followerId);

            return new Result<object> { IsSuccess = true, Message = "Followed.", StatusCode = 200 };
        }

        public async Task<Result<FollowListDto>> GetFollowersAsync(int userId, int page, int pageSize, int? currentUserId)
        {
            var user = await _userRepo.GetPublicByIdAsync(userId);
            if (user == null)
                return new Result<FollowListDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<FollowListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<FollowListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _followRepo.CountFollowersAsync(userId);
            var follows = await _followRepo.GetFollowersAsync(userId, page, pageSize);

            var followingIds = currentUserId.HasValue
                ? await _followRepo.GetFollowingIdsAsync(currentUserId.Value, follows.Select(f => f.FollowerId))
                : null;

            var result = new FollowListDto
            {
                Users = follows.Select(f => ToDto(f.Follower, followingIds, currentUserId)),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<FollowListDto> { Data = result, IsSuccess = true, Message = "Followers retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<FollowListDto>> GetFollowingAsync(int userId, int page, int pageSize, int? currentUserId)
        {
            var user = await _userRepo.GetPublicByIdAsync(userId);
            if (user == null)
                return new Result<FollowListDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<FollowListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<FollowListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _followRepo.CountFollowingAsync(userId);
            var follows = await _followRepo.GetFollowingAsync(userId, page, pageSize);

            var followingIds = currentUserId.HasValue
                ? await _followRepo.GetFollowingIdsAsync(currentUserId.Value, follows.Select(f => f.FollowingId))
                : null;

            var result = new FollowListDto
            {
                Users = follows.Select(f => ToDto(f.Following, followingIds, currentUserId)),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<FollowListDto> { Data = result, IsSuccess = true, Message = "Following retrieved successfully.", StatusCode = 200 };
        }

        // null = anonymous viewer or viewing myself in the list — same convention as
        // VideoService/UserService.GetMentorsAsync, so the client's rule stays
        // "only render the follow button when IsFollowing != null".
        private static FollowUserDto ToDto(User user, HashSet<int>? followingIds, int? currentUserId) => new FollowUserDto
        {
            Id = user.Id,
            Username = user.Username,
            ProfileImageUrl = user.Profile?.ProfileImageUrl,
            PrimaryCategory = user.Profile?.PrimaryCategory,
            SkillLevel = user.Profile?.SkillLevel,
            IsFollowing = followingIds == null || user.Id == currentUserId ? null : followingIds.Contains(user.Id)
        };
    }
}
