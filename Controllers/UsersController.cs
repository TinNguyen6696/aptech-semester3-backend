using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs;
using TalentShowcase.Api.DTOs.Achievements;
using TalentShowcase.Api.DTOs.Videos;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IUserService _userService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IVideoService _videoService;

        public UsersController(IUserService userService, IFileUploadService fileUploadService, IVideoService videoService)
        {
            _userService = userService;
            _fileUploadService = fileUploadService;
            _videoService = videoService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _userService.GetProfileAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicProfile(int id)
        {
            var result = await _userService.GetPublicProfileAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}/videos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPublicVideos(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _videoService.GetPublicVideosByUserAsync(id, page, pageSize);
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
        public async Task<IActionResult> AddAchievement([FromForm] CreateAchievementRequest request)
        {
            var result = await _userService.AddAchievementAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("me/achievements/{id}")]
        public async Task<IActionResult> UpdateAchievement(int id, [FromForm] UpdateAchievementRequest request)
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

        [HttpGet("me/videos")]
        public async Task<IActionResult> GetMyVideos()
        {
            var result = await _videoService.GetMyVideosAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("me/videos")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddVideo([FromForm] CreateVideoRequest request)
        {
            var result = await _videoService.AddVideoAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("me/videos/{id}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UpdateVideo(int id, [FromBody] UpdateVideoRequest request)
        {
            var result = await _videoService.UpdateVideoAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("me/videos/{id}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var result = await _videoService.DeleteVideoAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
