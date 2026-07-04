using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IFileUploadService _fileUploadService;

        public UsersController(IUserService userService, IFileUploadService fileUploadService)
        {
            _userService = userService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _userService.GetProfileAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
        {
            var result = await _userService.UpdateProfileAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("me/avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var upload = await _fileUploadService.UploadImageAsync(file);
            if (!upload.IsSuccess)
                return StatusCode(upload.StatusCode, upload);

            var result = await _userService.UpdateAvatarAsync(CurrentUserId, upload.Data);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("me/avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var result = await _userService.UpdateAvatarAsync(CurrentUserId, null);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("me/achievements")]
        public async Task<IActionResult> GetAchievements()
        {
            var result = await _userService.GetAchievementsAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("me/achievements")]
        public async Task<IActionResult> AddAchievement([FromBody] CreateAchievementRequest request)
        {
            var result = await _userService.AddAchievementAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("me/achievements/{id}")]
        public async Task<IActionResult> UpdateAchievement(int id, [FromBody] UpdateAchievementRequest request)
        {
            var result = await _userService.UpdateAchievementAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("me/achievements/{id}")]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            var result = await _userService.DeleteAchievementAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
