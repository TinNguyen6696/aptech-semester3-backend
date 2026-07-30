using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class VideoViewService : IVideoViewService
    {
        // SQL Server error numbers for a unique index/constraint violation.
        private static readonly int[] UniqueViolationErrorNumbers = { 2601, 2627 };

        private readonly IVideoViewRepository _viewRepo;
        private readonly IVideoRepository _videoRepo;

        public VideoViewService(IVideoViewRepository viewRepo, IVideoRepository videoRepo)
        {
            _viewRepo = viewRepo;
            _videoRepo = videoRepo;
        }

        public async Task<Result<int>> RecordViewAsync(int? userId, int videoId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<int> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var counted = true;

            try
            {
                await _viewRepo.AddAsync(new VideoView { VideoId = videoId, UserId = userId });
                await _viewRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && UniqueViolationErrorNumbers.Contains(sqlEx.Number))
            {
                // Same logged-in user viewing again — unique index rejected the duplicate row, ignore.
                // Any other DbUpdateException (connection failure, unrelated constraint, etc.) is not
                // caught here and bubbles up to ExceptionHandlingMiddleware as a real 500.
                counted = false;
            }

            // Always return the authoritative count, whether or not this call added a row. The client
            // has no way to predict the dedup outcome, so an optimistic +1 on its side would drift —
            // it should just render this number.
            var viewCount = await _viewRepo.CountByVideoIdAsync(videoId);

            return new Result<int>
            {
                Data = viewCount,
                IsSuccess = true,
                Message = counted ? "View recorded." : "View already counted.",
                StatusCode = 200
            };
        }
    }
}