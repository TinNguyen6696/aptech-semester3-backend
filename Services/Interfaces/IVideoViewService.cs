using TalentShowcase.Api.Common;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IVideoViewService
    {
        Task<Result<object>> RecordViewAsync(int? userId, int videoId);
    }
}