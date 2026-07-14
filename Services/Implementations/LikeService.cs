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
        private readonly ICommunityPostRepository _communityPostRepo;

        public LikeService(ILikeRepository likeRepo, IVideoRepository videoRepo, ICommunityPostRepository communityPostRepo)
        {
            _likeRepo = likeRepo;
            _videoRepo = videoRepo;
            _communityPostRepo = communityPostRepo;
        }

        public async Task<Result<object>> ToggleVideoLikeAsync(int userId, int videoId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);
            if (video == null)
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            return await ToggleAsync(userId, ReferenceTypes.Video, videoId);
        }

        public async Task<Result<object>> ToggleCommunityPostLikeAsync(int userId, int postId)
        {
            var post = await _communityPostRepo.GetByIdAsync(postId);
            if (post == null)
                return new Result<object> { IsSuccess = false, Message = "Post not found.", StatusCode = 404 };

            return await ToggleAsync(userId, ReferenceTypes.CommunityPost, postId);
        }

        private async Task<Result<object>> ToggleAsync(int userId, string referenceType, int referenceId)
        {
            var existing = await _likeRepo.GetAsync(referenceType, referenceId, userId);
            if (existing != null)
            {
                _likeRepo.Remove(existing);
                await _likeRepo.SaveChangesAsync();
                return new Result<object> { IsSuccess = true, Message = "Unliked.", StatusCode = 200 };
            }

            await _likeRepo.AddAsync(new Like { UserId = userId, ReferenceType = referenceType, ReferenceId = referenceId });
            await _likeRepo.SaveChangesAsync();
            return new Result<object> { IsSuccess = true, Message = "Liked.", StatusCode = 200 };
        }
    }
}