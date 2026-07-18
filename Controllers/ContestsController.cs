using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Contests;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class ContestsController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IContestService _contestService;

        public ContestsController(IContestService contestService)
        {
            _contestService = contestService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetContests([FromQuery] TalentCategory? category, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _contestService.GetContestsAsync(category, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContest(int id)
        {
            var result = await _contestService.GetContestByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateContest([FromBody] CreateContestRequest request)
        {
            var result = await _contestService.CreateContestAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContest(int id, [FromBody] UpdateContestRequest request)
        {
            var result = await _contestService.UpdateContestAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContest(int id)
        {
            var result = await _contestService.DeleteContestAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}/entries")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEntries(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _contestService.GetEntriesAsync(id, page, pageSize, CurrentUserIdOrNull);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id:int}/entries")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddEntry(int id, [FromBody] CreateContestEntryRequest request)
        {
            var result = await _contestService.AddEntryAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}/entries/{entryId:int}")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<IActionResult> WithdrawEntry(int id, int entryId)
        {
            var isAdmin = CurrentUserRole == nameof(UserRole.Admin);
            var result = await _contestService.WithdrawEntryAsync(CurrentUserId, id, entryId, isAdmin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id:int}/entries/{entryId:int}/vote")]
        [Authorize(Roles = "Member,Mentor,Recruiter")]
        public async Task<IActionResult> ToggleVote(int id, int entryId)
        {
            var result = await _contestService.ToggleVoteAsync(CurrentUserId, id, entryId);
            return StatusCode(result.StatusCode, result);
        }
    }
}