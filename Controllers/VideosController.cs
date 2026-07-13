using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Ratings;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class VideosController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IVideoService _videoService;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;
        private readonly IRatingService _ratingService;
        private readonly IVideoViewService _videoViewService;

        public VideosController(
            IVideoService videoService,
            ILikeService likeService,
            ICommentService commentService,
            IRatingService ratingService,
            IVideoViewService videoViewService)
        {
            _videoService = videoService;
            _likeService = likeService;
            _commentService = commentService;
            _ratingService = ratingService;
            _videoViewService = videoViewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicVideos([FromQuery] TalentCategory? category, [FromQuery] VideoSortBy sortBy = VideoSortBy.Newest, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _videoService.GetPublicVideosAsync(category, sortBy, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublicVideo(int id)
        {
            var result = await _videoService.GetPublicVideoByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/like")]
        [Authorize(Roles = "Member,Mentor")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var result = await _likeService.ToggleVideoLikeAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _commentService.GetVideoCommentsAsync(id, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/comments")]
        [Authorize(Roles = "Member,Mentor")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentRequest request)
        {
            var result = await _commentService.AddVideoCommentAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/rating")]
        [Authorize(Roles = "Member,Mentor")]
        public async Task<IActionResult> RateVideo(int id, [FromBody] RateVideoRequest request)
        {
            var result = await _ratingService.RateVideoAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordView(int id)
        {
            var result = await _videoViewService.RecordViewAsync(CurrentUserIdOrNull, id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
