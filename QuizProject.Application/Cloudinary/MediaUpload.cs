using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace QuizProject.Application.Cloudinary;

public class MediaUpload : IMediaUpload
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    
    public MediaUpload(CloudinaryDotNet.Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }
    
    public async Task<ImageUploadResult> UploadImageAsync(IFormFile image, string folder)
    {
        using var stream = image.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Folder = folder,
            PublicId = Guid.NewGuid().ToString()
        };
        
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
        {
            throw new Exception(result.Error.Message);
        }
        
        return result;
    }
}