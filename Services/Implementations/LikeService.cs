using TalentShowcase.Api.Common;
using TalentShowcase.Api.Models.Constants;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepo;
        private readonly IVideoRepository _videoRepo;

        public LikeService(ILikeRepository likeRepo, IVideoRepository videoRepo)
        {
            _likeRepo = likeRepo;
            _videoRepo = videoRepo;
        }

        public async Task<Result<object>> ToggleVideoLikeAsync(int userId, int videoId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            var existing = await _likeRepo.GetAsync(ReferenceTypes.Video, videoId, userId);
            if (existing != null)
            {
                _likeRepo.Remove(existing);
                await _likeRepo.SaveChangesAsync();
                return new Result<object> { IsSuccess = true, Message = "Unliked.", StatusCode = 200 };
            }

            await _likeRepo.AddAsync(new Like { UserId = userId, ReferenceType = ReferenceTypes.Video, ReferenceId = videoId });
            await _likeRepo.SaveChangesAsync();
            return new Result<object> { IsSuccess = true, Message = "Liked.", StatusCode = 200 };
        }
    }
}