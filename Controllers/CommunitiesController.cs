using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Communities;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class CommunitiesController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly ICommunityService _communityService;

        public CommunitiesController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommunities()
        {
            var result = await _communityService.GetCommunitiesAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommunity(int id)
        {
            var result = await _communityService.GetCommunityByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/posts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommunityPosts(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _communityService.GetCommunityPostsAsync(id, page, pageSize, CurrentUserIdOrNull);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/posts")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> AddCommunityPost(int id, [FromBody] CreateCommunityPostRequest request)
        {
            var result = await _communityService.AddCommunityPostAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}