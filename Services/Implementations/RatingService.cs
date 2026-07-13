using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Ratings;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private const int MinScore = 1;
        private const int MaxScore = 5;

        private readonly IRatingRepository _ratingRepo;
        private readonly IVideoRepository _videoRepo;

        public RatingService(IRatingRepository ratingRepo, IVideoRepository videoRepo)
        {
            _ratingRepo = ratingRepo;
            _videoRepo = videoRepo;
        }

        public async Task<Result<RatingDto>> RateVideoAsync(int userId, int videoId, RateVideoRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<RatingDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (request.Score is < MinScore or > MaxScore)
                return new Result<RatingDto> { IsSuccess = false, Message = $"Score must be between {MinScore} and {MaxScore}.", StatusCode = 400 };

            var rating = await _ratingRepo.GetByVideoAndUserAsync(videoId, userId);
            if (rating == null)
            {
                rating = new Rating { VideoId = videoId, UserId = userId, Score = request.Score!.Value };
                await _ratingRepo.AddAsync(rating);
            }
            else
            {
                rating.Score = request.Score!.Value;
                _ratingRepo.Update(rating);
            }

            await _ratingRepo.SaveChangesAsync();

            return new Result<RatingDto> { Data = ToDto(rating), IsSuccess = true, Message = "Rating saved successfully.", StatusCode = 200 };
        }

        private static RatingDto ToDto(Rating rating) => new RatingDto
        {
            Id = rating.Id,
            VideoId = rating.VideoId,
            Score = rating.Score,
            CreatedAt = rating.CreatedAt,
            UpdatedAt = rating.UpdatedAt
        };
    }
}