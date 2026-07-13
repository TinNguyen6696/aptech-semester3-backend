using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Repositories.Interfaces
{
    public interface IVideoViewRepository : IGenericRepository<VideoView>
    {
        Task<int> CountByVideoIdAsync(int videoId);
        Task<Dictionary<int, int>> CountByVideoIdsAsync(IEnumerable<int> videoIds);
    }
}