using ASP_shopgiay.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace ASP_shopgiay.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            // Cấu hình tài khoản Cloudinary
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // (Tùy chọn) Tự động cắt ảnh vuông 500x500
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        // (Bạn sẽ dùng hàm này khi làm chức năng Xóa)
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        public string GetPublicIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            try
            {
                // URL: .../upload/v12345/folder/my_image.jpg
                // 1. Lấy phần cuối: "folder/my_image.jpg"
                int lastSlash = imageUrl.LastIndexOf('/') + 1;
                string fileNameWithFormat = imageUrl.Substring(lastSlash);

                // 2. Bỏ phần version (nếu có): "v12345/"
                if (fileNameWithFormat.StartsWith("v") && char.IsDigit(fileNameWithFormat[1]))
                {
                    fileNameWithFormat = fileNameWithFormat.Substring(fileNameWithFormat.IndexOf('/') + 1);
                }

                // 3. Lấy tên file: "folder/my_image"
                int lastDot = fileNameWithFormat.LastIndexOf('.');
                if (lastDot < 0) return fileNameWithFormat; // Nếu không có đuôi file

                return fileNameWithFormat.Substring(0, lastDot);
            }
            catch
            {
                return null; // Nếu URL không hợp lệ
            }
        }
    }
}
