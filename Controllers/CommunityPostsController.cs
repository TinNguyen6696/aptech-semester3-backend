using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Communities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Route("api/community-posts")]
    public class CommunityPostsController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly ICommunityService _communityService;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;

        public CommunityPostsController(ICommunityService communityService, ILikeService likeService, ICommentService commentService)
        {
            _communityService = communityService;
            _likeService = likeService;
            _commentService = commentService;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdateCommunityPostRequest request)
        {
            var result = await _communityService.UpdateCommunityPostAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Mentor,Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var isAdmin = CurrentUserRole == nameof(UserRole.Admin);
            var result = await _communityService.DeleteCommunityPostAsync(CurrentUserId, id, isAdmin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/like")]
        [Authorize(Roles = "Member,Mentor")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var result = await _likeService.ToggleCommunityPostLikeAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _commentService.GetCommunityPostCommentsAsync(id, page, pageSize, CurrentUserIdOrNull);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/comments")]
        [Authorize(Roles = "Member,Mentor")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentRequest request)
        {
            var result = await _commentService.AddCommunityPostCommentAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}