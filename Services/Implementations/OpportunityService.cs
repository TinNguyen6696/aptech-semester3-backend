using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Comments;
using TalentShowcase.Api.DTOs.Opportunities;
using TalentShowcase.Api.Models.Constants;
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
        private readonly IOpportunityApplicationRepository _applicationRepo;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepo;
        private readonly IFollowRepository _followRepo;

        public OpportunityService(
            IOpportunityRepository opportunityRepo,
            IGenericRepository<Province> provinceRepo,
            IOpportunityApplicationRepository applicationRepo,
            INotificationService notificationService,
            IUserRepository userRepo,
            IFollowRepository followRepo)
        {
            _opportunityRepo = opportunityRepo;
            _provinceRepo = provinceRepo;
            _applicationRepo = applicationRepo;
            _notificationService = notificationService;
            _userRepo = userRepo;
            _followRepo = followRepo;
        }

        public async Task<Result<OpportunityListDto>> GetOpportunitiesAsync(TalentCategory? category, int? provinceId, int page, int pageSize, int? currentUserId)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (page < 1)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _opportunityRepo.CountPublicAsync(category, provinceId);
            var opportunities = (await _opportunityRepo.GetPublicAsync(category, provinceId, page, pageSize)).ToList();

            var appliedIds = currentUserId.HasValue
                ? await _applicationRepo.GetAppliedOpportunityIdsAsync(opportunities.Select(o => o.Id), currentUserId.Value)
                : null;

            var result = new OpportunityListDto
            {
                Opportunities = opportunities.Select(o => ToDto(o, appliedIds == null ? null : appliedIds.Contains(o.Id))),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<OpportunityListDto> { Data = result, IsSuccess = true, Message = "Opportunities retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<OpportunityDto>> GetOpportunityByIdAsync(int id, int? currentUserId)
        {
            var opportunity = await _opportunityRepo.GetByIdWithDetailsAsync(id);
            if (opportunity == null)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            bool? isApplied = currentUserId.HasValue
                ? await _applicationRepo.ExistsAsync(id, currentUserId.Value)
                : null;

            return new Result<OpportunityDto> { Data = ToDto(opportunity, isApplied), IsSuccess = true, Message = "Opportunity retrieved successfully.", StatusCode = 200 };
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
                Opportunities = opportunities.Select(o => ToDto(o)),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<OpportunityListDto> { Data = result, IsSuccess = true, Message = "Opportunities retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<OpportunityListDto>> GetOpportunitiesByUserAsync(int userId, int page, int pageSize, int? currentUserId)
        {
            if (page < 1)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<OpportunityListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _opportunityRepo.CountPublicByUserIdAsync(userId);
            var opportunities = (await _opportunityRepo.GetPublicByUserIdAsync(userId, page, pageSize)).ToList();

            var appliedIds = currentUserId.HasValue
                ? await _applicationRepo.GetAppliedOpportunityIdsAsync(opportunities.Select(o => o.Id), currentUserId.Value)
                : null;

            var result = new OpportunityListDto
            {
                Opportunities = opportunities.Select(o => ToDto(o, appliedIds == null ? null : appliedIds.Contains(o.Id))),
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

            if (request.ExpiresAt!.Value <= DateTime.UtcNow)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Expiry date must be in the future.", StatusCode = 400 };

            var opportunity = new Opportunity
            {
                PostedByUserId = userId,
                Category = request.Category!.Value,
                Title = request.Title,
                Description = request.Description,
                ProvinceId = request.ProvinceId!.Value,
                PostedAt = request.PostedAt!.Value,
                ExpiresAt = request.ExpiresAt!.Value
            };

            await _opportunityRepo.AddAsync(opportunity);
            await _opportunityRepo.SaveChangesAsync();

            var created = await _opportunityRepo.GetByIdWithDetailsAsync(opportunity.Id);

            var followerIds = await _followRepo.GetAllFollowerIdsAsync(userId);
            if (followerIds.Count > 0)
                await _notificationService.CreateManyAsync(
                    followerIds,
                    $"{created!.PostedByUser.Username} posted a new opportunity.",
                    ReferenceTypes.Opportunity,
                    opportunity.Id);

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

            if (request.ExpiresAt!.Value <= DateTime.UtcNow)
                return new Result<OpportunityDto> { IsSuccess = false, Message = "Expiry date must be in the future.", StatusCode = 400 };

            opportunity.Category = request.Category!.Value;
            opportunity.Title = request.Title;
            opportunity.Description = request.Description;
            opportunity.ProvinceId = request.ProvinceId!.Value;
            opportunity.PostedAt = request.PostedAt!.Value;
            opportunity.ExpiresAt = request.ExpiresAt!.Value;

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

        public async Task<Result<object>> ApplyAsync(int userId, int opportunityId)
        {
            var opportunity = await _opportunityRepo.GetByIdWithDetailsAsync(opportunityId);
            if (opportunity == null)
                return new Result<object> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            if (opportunity.ExpiresAt < DateTime.UtcNow)
                return new Result<object> { IsSuccess = false, Message = "This opportunity has expired.", StatusCode = 400 };

            if (await _applicationRepo.ExistsAsync(opportunityId, userId))
                return new Result<object> { IsSuccess = false, Message = "You have already applied to this opportunity.", StatusCode = 400 };

            await _applicationRepo.AddAsync(new OpportunityApplication { OpportunityId = opportunityId, ApplicantUserId = userId });
            await _applicationRepo.SaveChangesAsync();

            await _notificationService.CreateAsync(
                userId,
                "You applied successfully. Waiting for the recruiter to review your profile.",
                ReferenceTypes.Opportunity,
                opportunityId);

            var applicant = await _userRepo.GetByIdAsync(userId);
            if (applicant != null)
                await _notificationService.CreateAsync(
                    opportunity.PostedByUserId,
                    $"{applicant.Username} applied to your opportunity \"{opportunity.Title}\".",
                    ReferenceTypes.Opportunity,
                    opportunityId);

            return new Result<object> { IsSuccess = true, Message = "Applied successfully.", StatusCode = 201 };
        }

        public async Task<Result<OpportunityApplicationListDto>> GetApplicantsAsync(int recruiterUserId, int opportunityId, int page, int pageSize)
        {
            var opportunity = await _opportunityRepo.GetByIdAsync(opportunityId);
            if (opportunity == null || opportunity.PostedByUserId != recruiterUserId)
                return new Result<OpportunityApplicationListDto> { IsSuccess = false, Message = "Opportunity not found.", StatusCode = 404 };

            if (page < 1)
                return new Result<OpportunityApplicationListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<OpportunityApplicationListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _applicationRepo.CountByOpportunityIdAsync(opportunityId);
            var applications = await _applicationRepo.GetByOpportunityIdAsync(opportunityId, page, pageSize);

            var result = new OpportunityApplicationListDto
            {
                Applications = applications.Select(ToDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<OpportunityApplicationListDto> { Data = result, IsSuccess = true, Message = "Applicants retrieved successfully.", StatusCode = 200 };
        }

        private static OpportunityApplicationDto ToDto(OpportunityApplication application) => new OpportunityApplicationDto
        {
            Id = application.Id,
            OpportunityId = application.OpportunityId,
            AppliedAt = application.CreatedAt,
            Applicant = new OpportunityApplicantDto
            {
                Id = application.ApplicantUser.Id,
                Username = application.ApplicantUser.Username,
                FirstName = application.ApplicantUser.Profile?.FirstName,
                LastName = application.ApplicantUser.Profile?.LastName,
                ProfileImageUrl = application.ApplicantUser.Profile?.ProfileImageUrl,
                PrimaryCategory = application.ApplicantUser.Profile?.PrimaryCategory,
                SkillLevel = application.ApplicantUser.Profile?.SkillLevel
            }
        };

        private static OpportunityDto ToDto(Opportunity opportunity, bool? isApplied = null) => new OpportunityDto
        {
            Id = opportunity.Id,
            Category = opportunity.Category,
            Title = opportunity.Title,
            Description = opportunity.Description,
            ProvinceId = opportunity.ProvinceId,
            ProvinceName = opportunity.Province.Name,
            PostedAt = opportunity.PostedAt,
            ExpiresAt = opportunity.ExpiresAt,
            IsExpired = opportunity.ExpiresAt < DateTime.UtcNow,
            IsApplied = isApplied,
            CreatedAt = opportunity.CreatedAt,
            PostedBy = new CommentAuthorDto
            {
                Id = opportunity.PostedByUser.Id,
                Username = opportunity.PostedByUser.Username,
                FirstName = opportunity.PostedByUser.Profile?.FirstName,
                LastName = opportunity.PostedByUser.Profile?.LastName,
                ProfileImageUrl = opportunity.PostedByUser.Profile?.ProfileImageUrl
            }
        };
    }
}