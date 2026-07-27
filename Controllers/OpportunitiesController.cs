using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentShowcase.Api.DTOs.Opportunities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Controllers
{
    public class OpportunitiesController : BaseApiController
    {
        private const int DefaultPageSize = 10;

        private readonly IOpportunityService _opportunityService;

        public OpportunitiesController(IOpportunityService opportunityService)
        {
            _opportunityService = opportunityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetOpportunities([FromQuery] TalentCategory? category, [FromQuery] int? provinceId, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _opportunityService.GetOpportunitiesAsync(category, provinceId, page, pageSize, CurrentUserIdOrNull);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetMyOpportunities([FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _opportunityService.GetMyOpportunitiesAsync(CurrentUserId, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOpportunity(int id)
        {
            var result = await _opportunityService.GetOpportunityByIdAsync(id, CurrentUserIdOrNull);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateOpportunity([FromBody] CreateOpportunityRequest request)
        {
            var result = await _opportunityService.CreateOpportunityAsync(CurrentUserId, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateOpportunity(int id, [FromBody] UpdateOpportunityRequest request)
        {
            var result = await _opportunityService.UpdateOpportunityAsync(CurrentUserId, id, request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> DeleteOpportunity(int id)
        {
            var isAdmin = CurrentUserRole == nameof(UserRole.Admin);
            var result = await _opportunityService.DeleteOpportunityAsync(CurrentUserId, id, isAdmin);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id:int}/apply")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Apply(int id)
        {
            var result = await _opportunityService.ApplyAsync(CurrentUserId, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:int}/applicants")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetApplicants(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = DefaultPageSize)
        {
            var result = await _opportunityService.GetApplicantsAsync(CurrentUserId, id, page, pageSize);
            return StatusCode(result.StatusCode, result);
        }
    }
}