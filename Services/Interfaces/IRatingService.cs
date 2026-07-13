using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Ratings;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IRatingService
    {
        Task<Result<RatingDto>> RateVideoAsync(int userId, int videoId, RateVideoRequest request);
    }
}