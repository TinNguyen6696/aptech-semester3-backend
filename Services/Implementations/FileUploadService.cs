using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<Result<string>> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new Result<string> { IsSuccess = false, Message = "No file provided.", StatusCode = 400 };

            if (!AllowedContentTypes.Contains(file.ContentType))
                return new Result<string> { IsSuccess = false, Message = "Invalid file type. Allowed: JPEG, PNG, WEBP.", StatusCode = 400 };

            if (file.Length > MaxFileSizeBytes)
                return new Result<string> { IsSuccess = false, Message = "File too large. Max size is 5MB.", StatusCode = 400 };

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new Result<string> { Data = $"/uploads/{fileName}", IsSuccess = true, Message = "Image uploaded successfully.", StatusCode = 200 };
        }
    }
}
