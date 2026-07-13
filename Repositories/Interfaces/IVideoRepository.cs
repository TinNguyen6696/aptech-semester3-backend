using TalentShowcase.Api.Models.Entities;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IVideoRepository : IGenericRepository<Video>
    {
        Task<IEnumerable<Video>> GetByUserIdAsync(int userId);
        Task<int> CountByUserIdAsync(int userId);
        Task<IEnumerable<Video>> GetPublicAsync(TalentCategory? category, VideoSortBy sortBy, int page, int pageSize);
        Task<int> CountPublicAsync(TalentCategory? category);
        Task<IEnumerable<Video>> GetPublicByUserIdAsync(int userId, int page, int pageSize);
        Task<int> CountPublicByUserIdAsync(int userId);
        Task<Video?> GetPublicByIdAsync(int id);
    }
}
