using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class VideosController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicVideos([FromQuery] TalentCategory? category, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _videoService.GetPublicVideosAsync(category, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublicVideo(int id)
        {
            var result = await _videoService.GetPublicVideoByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
