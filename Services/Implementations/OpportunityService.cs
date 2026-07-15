using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Opportunities;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class OpportunityService : IOpportunityService
    {
        private const int MaxPageSize = 10;

        private readonly IOpportunityRepository _opportunityRepo;
        private readonly IGenericRepository<Province> _provinceRepo;

        public OpportunityService(IOpportunityRepository opportunityRepo, IGenericRepository<Province> provinceRepo)
        {
            _opportunityRepo = opportunityRepo;
            _provinceRepo = provinceRepo;
        }

        public async Task<Result<OpportunityListDto>> GetOpportunitiesAsync(TalentCategory? category, int? provinceId, int page, int pageSize)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (page < 1)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _opportunityRepo.CountPublicAsync(category, provinceId);
            var opportunities = await _opportunityRepo.GetPublicAsync(category, provinceId, page, pageSize);

            var result = new OpportunityListDto
            {
                Opportunities = opportunities.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<OpportunityListDto> { Data = result, IsSuccess = true, Message = "Opportunities retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<OpportunityDto>> GetOpportunityByIdAsync(int id)
        {
            var opportunity = await _opportunityRepo.GetByIdWithDetailsAsync(id);
            if (opportunity == null)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            return new Result<OpportunityDto> { Data = ToDto(opportunity), IsSuccess = true, Message = "Opportunity retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<OpportunityListDto>> GetMyOpportunitiesAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _opportunityRepo.CountByUserIdAsync(userId);
            var opportunities = await _opportunityRepo.GetByUserIdAsync(userId, page, pageSize);

            var result = new OpportunityListDto
            {
                Opportunities = opportunities.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<OpportunityListDto> { Data = result, IsSuccess = true, Message = "Opportunities retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<OpportunityDto>> CreateOpportunityAsync(int userId, CreateOpportunityRequest request)
        {
            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (await _provinceRepo.GetByIdAsync(request.ProvinceId!.Value) == null)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Invalid province.", StatusCode = 400 };

            var opportunity = new Opportunity
            {
                PostedByUserId = userId,
                Category = request.Category!.Value,
                Title = request.Title,
                Description = request.Description,
                ProvinceId = request.ProvinceId!.Value
            };

            await _opportunityRepo.AddAsync(opportunity);
            await _opportunityRepo.SaveChangesAsync();

            var created = await _opportunityRepo.GetByIdWithDetailsAsync(opportunity.Id);
            return new Result<OpportunityDto> { Data = ToDto(created!), IsSuccess = true, Message = "Opportunity posted successfully.", StatusCode = 201 };
        }

        public async Task<Result<OpportunityDto>> UpdateOpportunityAsync(int userId, int opportunityId, UpdateOpportunityRequest request)
        {
            var opportunity = await _opportunityRepo.GetByIdWithDetailsAsync(opportunityId);
            if (opportunity == null || opportunity.PostedByUserId != userId)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (await _provinceRepo.GetByIdAsync(request.ProvinceId!.Value) == null)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Invalid province.", StatusCode = 400 };

            opportunity.Category = request.Category!.Value;
            opportunity.Title = request.Title;
            opportunity.Description = request.Description;
            opportunity.ProvinceId = request.ProvinceId!.Value;

            _opportunityRepo.Update(opportunity);
            await _opportunityRepo.SaveChangesAsync();

            var updated = await _opportunityRepo.GetByIdWithDetailsAsync(opportunityId);
            return new Result<OpportunityDto> { Data = ToDto(updated!), IsSuccess = true, Message = "Opportunity updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteOpportunityAsync(int userId, int opportunityId, bool isAdmin)
        {
            var opportunity = await _opportunityRepo.GetByIdAsync(opportunityId);
            if (opportunity == null || (opportunity.PostedByUserId != userId && !isAdmin))
                return new Result<object> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            _opportunityRepo.Remove(opportunity);
            await _opportunityRepo.SaveChangesAsync();

            return new Result<object> { IsSuccess = true, Message = "Opportunity deleted successfully.", StatusCode = 200 };
        }

        private static OpportunityDto ToDto(Opportunity opportunity) => new OpportunityDto
        {
            Id = opportunity.Id,
            Category = opportunity.Category,
            Title = opportunity.Title,
            Description = opportunity.Description,
            ProvinceId = opportunity.ProvinceId,
            ProvinceName = opportunity.Province.Name,
            CreatedAt = opportunity.CreatedAt,
            PostedBy = new CommentAuthorDto
            {
                Id = opportunity.PostedByUser.Id,
                Username = opportunity.PostedByUser.Username,
                ProfileImageUrl = opportunity.PostedByUser.Profile?.ProfileImageUrl
            }
        };
    }
}