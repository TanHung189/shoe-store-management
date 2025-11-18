using CloudinaryDotNet.Actions;

namespace ASP_shopgiay.Services
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        string GetPublicIdFromUrl(string imageUrl);
    }
}
