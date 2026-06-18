using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Authly.Services.Dtos
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, params string[] subFolder);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
