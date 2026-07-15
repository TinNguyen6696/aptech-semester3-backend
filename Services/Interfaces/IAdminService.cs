using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Admin;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<Result<AdminDashboardDto>> GetDashboardAsync();
        Task<Result<AdminUserListDto>> GetUsersAsync(UserRole? role, int page, int pageSize);
        Task<Result<AdminUserDto>> GetUserByIdAsync(int id);
        Task<Result<AdminUserDto>> ToggleUserActiveAsync(int currentUserId, int id);
        Task<Result<AdminVideoListDto>> GetVideosAsync(int page, int pageSize);
        Task<Result<AdminCommentListDto>> GetCommentsAsync(int page, int pageSize);
    }
}