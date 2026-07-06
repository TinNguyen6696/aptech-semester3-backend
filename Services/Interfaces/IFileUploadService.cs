using Microsoft.AspNetCore.Http;
using TalentShowcase.Api.Common;

namespace TalentShowcase.Api.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<Result<string>> UploadImageAsync(IFormFile file);
        Task<Result<string>> UploadVideoAsync(IFormFile file);
    }
}
