using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace QuizProject.Application.Cloudinary;

public interface IMediaUpload
{
    Task<ImageUploadResult> UploadImageAsync(IFormFile image, string folder);
}