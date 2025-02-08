using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        // Add Methods
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        // Delete Methods
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}