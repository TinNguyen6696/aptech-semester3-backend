using TalentShowcase.Api.Common;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IVideoViewService
    {
        Task<Result<int>> RecordViewAsync(int? userId, int videoId);
    }
}