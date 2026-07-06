using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Videos;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IVideoService
    {
        Task<Result<IEnumerable<VideoDto>>> GetMyVideosAsync(int userId);
        Task<Result<VideoDto>> AddVideoAsync(int userId, CreateVideoRequest request);
        Task<Result<VideoDto>> UpdateVideoAsync(int userId, int videoId, UpdateVideoRequest request);
        Task<Result<object>> DeleteVideoAsync(int userId, int videoId);
        Task<Result<PublicVideoListDto>> GetPublicVideosAsync(TalentCategory? category, int page, int pageSize);
        Task<Result<PublicVideoDto>> GetPublicVideoByIdAsync(int id);
    }
}
