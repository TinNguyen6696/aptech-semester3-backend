using TalentShowcase.Api.Common;
using TalentShowcase.Api.DTOs.Videos;
using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;
using TalentShowcase.Api.Repositories.Interfaces;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class VideoService : IVideoService
    {
        private const int MaxVideosPerUser = 3;
        private const int MaxPageSize = 10;

        private readonly IVideoRepository _videoRepo;
        private readonly IFileUploadService _fileUploadService;

        public VideoService(IVideoRepository videoRepo, IFileUploadService fileUploadService)
        {
            _videoRepo = videoRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<Result<IEnumerable<VideoDto>>> GetMyVideosAsync(int userId)
        {
            var videos = await _videoRepo.GetByUserIdAsync(userId);
            var dtos = videos.OrderByDescending(v => v.CreatedAt).Select(ToDto);

            return new Result<IEnumerable<VideoDto>> { Data = dtos, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<VideoDto>> AddVideoAsync(int userId, CreateVideoRequest request)
        {
            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.Visibility!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid visibility.", StatusCode = 400 };

            var currentCount = await _videoRepo.CountByUserIdAsync(userId);
            if (currentCount >= MaxVideosPerUser)
                return new Result<VideoDto> { IsSuccess = false, Message = $"Maximum of {MaxVideosPerUser} videos reached. Delete one before adding another.", StatusCode = 400 };

            var upload = await _fileUploadService.UploadVideoAsync(request.File);
            if (!upload.IsSuccess)
                return new Result<VideoDto> { IsSuccess = false, Message = upload.Message, StatusCode = upload.StatusCode };

            var video = new Video
            {
                UserId = userId,
                Category = request.Category!.Value,
                Title = request.Title,
                Description = request.Description,
                VideoUrl = upload.Data!,
                Visibility = request.Visibility!.Value
            };

            await _videoRepo.AddAsync(video);
            await _videoRepo.SaveChangesAsync();

            return new Result<VideoDto> { Data = ToDto(video), IsSuccess = true, Message = "Video added successfully.", StatusCode = 201 };
        }

        public async Task<Result<VideoDto>> UpdateVideoAsync(int userId, int videoId, UpdateVideoRequest request)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);

            if (video == null || video.UserId != userId)
                return new Result<VideoDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            if (!Enum.IsDefined(request.Category!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (!Enum.IsDefined(request.Visibility!.Value))
                return new Result<VideoDto> { IsSuccess = false, Message = "Invalid visibility.", StatusCode = 400 };

            video.Category = request.Category!.Value;
            video.Title = request.Title;
            video.Description = request.Description;
            video.Visibility = request.Visibility!.Value;

            _videoRepo.Update(video);
            await _videoRepo.SaveChangesAsync();

            return new Result<VideoDto> { Data = ToDto(video), IsSuccess = true, Message = "Video updated successfully.", StatusCode = 200 };
        }

        public async Task<Result<object>> DeleteVideoAsync(int userId, int videoId)
        {
            var video = await _videoRepo.GetByIdAsync(videoId);

            if (video == null || video.UserId != userId)
                return new Result<object> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            _videoRepo.Remove(video);
            await _videoRepo.SaveChangesAsync();

            _fileUploadService.DeleteFile(video.VideoUrl);
            _fileUploadService.DeleteFile(video.ThumbnailUrl);

            return new Result<object> { IsSuccess = true, Message = "Video deleted successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoListDto>> GetPublicVideosAsync(TalentCategory? category, int page, int pageSize)
        {
            if (category.HasValue && !Enum.IsDefined(category.Value))
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Invalid category.", StatusCode = 400 };

            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicAsync(category);
            var videos = await _videoRepo.GetPublicAsync(category, page, pageSize);

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(ToPublicDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoListDto>> GetPublicVideosByUserAsync(int userId, int page, int pageSize)
        {
            if (page < 1)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = "Page must be at least 1.", StatusCode = 400 };

            if (pageSize < 1 || pageSize > MaxPageSize)
                return new Result<PublicVideoListDto> { IsSuccess = false, Message = $"Page size must be between 1 and {MaxPageSize}.", StatusCode = 400 };

            var totalCount = await _videoRepo.CountPublicByUserIdAsync(userId);
            var videos = await _videoRepo.GetPublicByUserIdAsync(userId, page, pageSize);

            var result = new PublicVideoListDto
            {
                Videos = videos.Select(ToPublicDto),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return new Result<PublicVideoListDto> { Data = result, IsSuccess = true, Message = "Videos retrieved successfully.", StatusCode = 200 };
        }

        public async Task<Result<PublicVideoDto>> GetPublicVideoByIdAsync(int id)
        {
            var video = await _videoRepo.GetPublicByIdAsync(id);

            if (video == null)
                return new Result<PublicVideoDto> { IsSuccess = false, Message = "Video not found.", StatusCode = 404 };

            return new Result<PublicVideoDto> { Data = ToPublicDto(video), IsSuccess = true, Message = "Video retrieved successfully.", StatusCode = 200 };
        }

        private static VideoDto ToDto(Video video) => new VideoDto
        {
            Id = video.Id,
            Category = video.Category,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            Visibility = video.Visibility,
            CreatedAt = video.CreatedAt
        };

        private static PublicVideoDto ToPublicDto(Video video) => new PublicVideoDto
        {
            Id = video.Id,
            Category = video.Category,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            CreatedAt = video.CreatedAt,
            Owner = new VideoOwnerDto
            {
                Id = video.User.Id,
                Username = video.User.Username,
                ProfileImageUrl = video.User.Profile?.ProfileImageUrl,
                PrimaryCategory = video.User.Profile?.PrimaryCategory,
                SkillLevel = video.User.Profile?.SkillLevel
            }
        };
    }
}
