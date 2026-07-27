using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Opportunities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IOpportunityService
    {
        Task<Result<OpportunityListDto>> GetOpportunitiesAsync(TalentCategory? category, int? provinceId, int page, int pageSize, int? currentUserId);
        Task<Result<OpportunityDto>> GetOpportunityByIdAsync(int id, int? currentUserId);
        Task<Result<OpportunityListDto>> GetMyOpportunitiesAsync(int userId, int page, int pageSize);
        Task<Result<OpportunityListDto>> GetOpportunitiesByUserAsync(int userId, int page, int pageSize, int? currentUserId);
        Task<Result<OpportunityDto>> CreateOpportunityAsync(int userId, CreateOpportunityRequest request);
        Task<Result<OpportunityDto>> UpdateOpportunityAsync(int userId, int opportunityId, UpdateOpportunityRequest request);
        Task<Result<object>> DeleteOpportunityAsync(int userId, int opportunityId, bool isAdmin);
        Task<Result<object>> ApplyAsync(int userId, int opportunityId);
        Task<Result<OpportunityApplicationListDto>> GetApplicantsAsync(int recruiterUserId, int opportunityId, int page, int pageSize);
    }
}