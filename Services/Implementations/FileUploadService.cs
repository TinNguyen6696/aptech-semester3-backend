using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TalentShowcase.Api.Common;
using TalentShowcase.Api.Services.Interfaces;

namespace TalentShowcase.Api.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private static readonly Dictionary<string, string> AllowedImageTypes = new()
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp"
        };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;

        private static readonly Dictionary<string, string> AllowedVideoTypes = new()
        {
            ["video/mp4"] = ".mp4"
        };
        private const long MaxVideoSizeBytes = 300 * 1024 * 1024;

        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<Result<string>> UploadImageAsync(IFormFile file) =>
            SaveAsync(file, AllowedImageTypes, MaxImageSizeBytes, "images", "Invalid file type. Allowed: JPEG, PNG, WEBP.", "File too large. Max size is 5MB.");

        public Task<Result<string>> UploadVideoAsync(IFormFile file) =>
            SaveAsync(file, AllowedVideoTypes, MaxVideoSizeBytes, "videos", "Invalid file type. Allowed: MP4.", "File too large. Max size is 300MB.");

        private async Task<Result<string>> SaveAsync(IFormFile file, Dictionary<string, string> allowedContentTypes, long maxSizeBytes, string subfolder, string invalidTypeMessage, string tooLargeMessage)
        {
            if (file == null || file.Length == 0)
                return new Result<string> { IsSuccess = false, Message = "No file provided.", StatusCode = 400 };

            if (!allowedContentTypes.TryGetValue(file.ContentType, out var extension))
                return new Result<string> { IsSuccess = false, Message = invalidTypeMessage, StatusCode = 400 };

            if (file.Length > maxSizeBytes)
                return new Result<string> { IsSuccess = false, Message = tooLargeMessage, StatusCode = 400 };

            var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsPath = Path.Combine(webRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new Result<string> { Data = $"/uploads/{subfolder}/{fileName}", IsSuccess = true, Message = "File uploaded successfully.", StatusCode = 200 };
        }

        public void DeleteFile(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsRoot = Path.GetFullPath(Path.Combine(webRootPath, "uploads"));

            var fullPath = Path.GetFullPath(Path.Combine(webRootPath, fileUrl.TrimStart('/')));

            // Guard against path traversal: only ever delete inside the uploads folder.
            if (!fullPath.StartsWith(uploadsRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                return;

            try
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
            catch (IOException)
            {
                // Best-effort cleanup: the DB record is already gone, so a locked/missing
                // file must not fail the request. Leaving an orphan is acceptable here.
            }
            catch (UnauthorizedAccessException)
            {
                // Same as above.
            }
        }
    }
}
