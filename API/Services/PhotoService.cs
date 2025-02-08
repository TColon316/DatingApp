using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            // Variable to store the upload results
            var uploadResult = new ImageUploadResult();

            // Confirm there are images to be uploaded
            if (file.Length > 0)
            {
                // Open up a request to read the files
                using var stream = file.OpenReadStream();

                // Create the Upload Parameters
                var uploadParams = new ImageUploadParams
                {
                    // Grab the file being uploaded
                    File = new FileDescription(file.FileName, stream),

                    // Set the image dimensions, crop it into a square and then focus on the face in the image
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),

                    // Folder the images will be uploaded to
                    Folder = "da-net8"
                };

                // Upload to Cloudinary
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            // Return the upload results
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}