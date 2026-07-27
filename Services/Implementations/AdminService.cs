using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Admin;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private const int MaxPageSize = 10;
        private const int RecentDays = 7;
        private const int RecentUsersCount = 5;

        private readonly IUserRepository _userRepo;
        private readonly IVideoRepository _videoRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IContestRepository _contestRepo;
        private readonly IOpportunityRepository _opportunityRepo;
        private readonly ICommunityPostRepository _communityPostRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IVideoViewRepository _videoViewRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IMessageRepository _messageRepo;

        public AdminService(
            IUserRepository userRepo,
            IVideoRepository videoRepo,
            ICommentRepository commentRepo,
            IContestRepository contestRepo,
            IOpportunityRepository opportunityRepo,
            ICommunityPostRepository communityPostRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IVideoViewRepository videoViewRepo,
            ILikeRepository likeRepo,
            IMessageRepository messageRepo)
        {
            _userRepo = userRepo;
            _videoRepo = videoRepo;
            _commentRepo = commentRepo;
            _contestRepo = contestRepo;
            _opportunityRepo = opportunityRepo;
            _communityPostRepo = communityPostRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _videoViewRepo = videoViewRepo;
            _likeRepo = likeRepo;
            _messageRepo = messageRepo;
        }

        public async Task<Result<AdminDashboardDto>> GetDashboardAsync()
        {
            var roleCounts = await _userRepo.CountByRoleAsync();
            var totalUsers = roleCounts.Values.Sum();
            var activeUsers = await _userRepo.CountActiveAsync();

            var weekAgo = DateTime.UtcNow.AddDays(-RecentDays);
            var monthAgo = DateTime.UtcNow.AddMonths(-1);
            var newUsersThisWeek = await _userRepo.CountCreatedSinceAsync(weekAgo);
            var newUsersThisMonth = await _userRepo.CountCreatedSinceAsync(monthAgo);
            var newVideosThisWeek = await _videoRepo.CountCreatedSinceAsync(weekAgo);

            var totalVideos = await _videoRepo.CountAllAsync();
            var totalViews = await _videoViewRepo.CountAllAsync();
            var totalLikes = await _likeRepo.CountByReferenceTypeAsync(ReferenceTypes.Video);

            var totalContests = await _contestRepo.CountPublicAsync(null);
            var endedContests = await _contestRepo.CountEndedAsync();

            var totalOpportunities = await _opportunityRepo.CountPublicAsync(null, null);
            var totalCommunityPosts = await _communityPostRepo.CountAllAsync();
            var totalMessages = await _messageRepo.CountAllAsync();

            var recentUsers = await _userRepo.GetAllPagedAsync(null, 1, RecentUsersCount);

            var dto = new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                BannedUsers = totalUsers - activeUsers,
                MemberCount = roleCounts.GetValueOrDefault(UserRole.Member),
                MentorCount = roleCounts.GetValueOrDefault(UserRole.Mentor),
                RecruiterCount = roleCounts.GetValueOrDefault(UserRole.Recruiter),
                AdminCount = roleCounts.GetValueOrDefault(UserRole.Admin),
                TotalVideos = totalVideos,
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                TotalContests = totalContests,
                EndedContests = endedContests,
                TotalOpportunities = totalOpportunities,
                TotalCommunityPosts = totalCommunityPosts,
                TotalMessages = totalMessages,
                NewUsersLast7Days = newUsersThisWeek,
                NewUsersThisMonth = newUsersThisMonth,
                NewVideosLast7Days = newVideosThisWeek,
                RecentUsers = recentUsers.Select(ToDto)
            };

            return new Result<AdminDashboardDto> { Data = dto, IsSuccess = true, Message = "Dashboard retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<AdminUserListDto>> GetUsersAsync(UserRole? role, int page, int pageSize)
        {
            if (role.HasValue && !Enum.IsDefined(role.Value))
                return new Result<AdminUserListDto> { IsSuccess = false, Message = "Invalid role.", StatusCode = 400 };

            if (page < 1)
                return new Result<AdminUserListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<AdminUserListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _userRepo.CountAllAsync(role);
            var users = await _userRepo.GetAllPagedAsync(role, page, pageSize);

            var result = new AdminUserListDto
            {
                Users = users.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<AdminUserListDto> { Data = result, IsSuccess = true, Message = "Users retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<AdminUserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdWithProfileAsync(id);
            if (user == null)
                return new Result<AdminUserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            return new Result<AdminUserDto> { Data = ToDto(user), IsSuccess = true, Message = "User retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<AdminUserDto>> ToggleUserActiveAsync(int currentUserId, int id)
        {
            if (id == currentUserId)
                return new Result<AdminUserDto> { IsSuccess = false, Message = "You cannot deactivate your own account.", StatusCode = 400 };

            var user = await _userRepo.GetByIdWithProfileAsync(id);
            if (user == null)
                return new Result<AdminUserDto> { IsSuccess = false, Message = "User not found.", StatusCode = 404 };

            user.IsActive = !user.IsActive;
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            // Deactivating revokes all active sessions too — otherwise a user with a still-valid
            // refresh token could keep refreshing their access token and never actually be locked
            // out. New logins are already blocked by the IsActive check in AuthService.LoginAsync;
            // this closes the loop for anyone already logged in. Reactivating needs no extra step,
            // the user just logs in again normally.
            if (!user.IsActive)
            {
                var tokens = await _refreshTokenRepo.GetActiveByUserIdAsync(user.Id);
                foreach (var token in tokens)
                {
                    token.Revoked = true;
                    _refreshTokenRepo.Update(token);
                }
                await _refreshTokenRepo.SaveChangesAsync();
            }

            var message = user.IsActive ? "User activated successfully." : "User deactivated successfully.";
            return new Result<AdminUserDto> { Data = ToDto(user), IsSuccess = true, Message = message, StatusCode = 200 };
        }

        public async Task<Result<AdminVideoListDto>> GetVideosAsync(int page, int pageSize)
        {
            if (page < 1)
                return new Result<AdminVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<AdminVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountAllAsync();
            var videos = await _videoRepo.GetAllPagedAsync(page, pageSize);

            var result = new AdminVideoListDto
            {
                Videos = videos.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<AdminVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<AdminCommentListDto>> GetCommentsAsync(int page, int pageSize)
        {
            if (page < 1)
                return new Result<AdminCommentListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<AdminCommentListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _commentRepo.CountAllAsync();
            var comments = await _commentRepo.GetAllPagedAsync(page, pageSize);

            var result = new AdminCommentListDto
            {
                Comments = comments.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<AdminCommentListDto> { Data = result, IsSuccess = true, Message = "Comments retrieved successfully.", StatusCode = 200 };
        }

        private static AdminUserDto ToDto(User user) => new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            FirstName = user.Profile?.FirstName,
            LastName = user.Profile?.LastName,
            ProfileImageUrl = user.Profile?.ProfileImageUrl,
            CreatedAt = user.CreatedAt
        };

        private static AdminVideoDto ToDto(Video video) => new AdminVideoDto
        {
            Id = video.Id,
            Title = video.Title,
            VideoUrl = video.VideoUrl,
            Category = video.Category,
            Visibility = video.Visibility,
            CreatedAt = video.CreatedAt,
            Owner = new CommentAuthorDto
            {
                Id = video.User.Id,
                Username = video.User.Username,
                FirstName = video.User.Profile?.FirstName,
                LastName = video.User.Profile?.LastName,
                ProfileImageUrl = video.User.Profile?.ProfileImageUrl
            }
        };

        private static AdminCommentDto ToDto(Comment comment) => new AdminCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            ReferenceType = comment.ReferenceType,
            ReferenceId = comment.ReferenceId,
            CreatedAt = comment.CreatedAt,
            Author = new CommentAuthorDto
            {
                Id = comment.User.Id,
                Username = comment.User.Username,
                FirstName = comment.User.Profile?.FirstName,
                LastName = comment.User.Profile?.LastName,
                ProfileImageUrl = comment.User.Profile?.ProfileImageUrl
            }
        };
    }
}