using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Messages;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _messageService.GetConversationsAsync(CurrentUserId, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _messageService.GetUnreadCountAsync(CurrentUserId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetConversation(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _messageService.GetConversationAsync(CurrentUserId, userId, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Member,Mentor,Recruiter")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var result = await _messageService.SendMessageAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{userId:int}/read")]
        public async Task<IActionResult> MarkConversationAsRead(int userId)
        {
            var result = await _messageService.MarkConversationAsReadAsync(CurrentUserId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}