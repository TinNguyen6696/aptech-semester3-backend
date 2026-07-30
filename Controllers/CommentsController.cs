using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Authorize]
    public class CommentsController : BaseApiController
    {
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;

        public CommentsController(ICommentService commentService, ILikeService likeService)
        {
            _commentService = commentService;
            _likeService = likeService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
        {
            var result = await _commentService.UpdateCommentAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var isAdmin = CurrentUserRole == nameof(UserRole.Admin);
            var result = await _commentService.DeleteCommentAsync(CurrentUserId, id, isAdmin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id}/like")]
        [Authorize(Roles = "Member,Mentor,Recruiter")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var result = await _likeService.ToggleCommentLikeAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }
    }
}