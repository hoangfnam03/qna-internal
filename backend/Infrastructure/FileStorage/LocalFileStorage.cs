using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.FileStorage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly LocalFileStorageOptions _opt;

        public LocalFileStorage(IOptions<LocalFileStorageOptions> opt)
        {
            _opt = opt.Value;
        }

        public async Task<StoredFileResult> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken ct)
        {
            var ext = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = GuessExt(contentType);

            var datePath = DateTime.UtcNow.ToString("yyyy/MM");
            var fileName = $"{Guid.NewGuid():N}{ext}";

            var root = _opt.RootPath;
            if (!Path.IsPathRooted(root))
                root = Path.Combine(Directory.GetCurrentDirectory(), root);

            var dir = Path.Combine(root, datePath);
            Directory.CreateDirectory(dir);

            var fullPath = Path.Combine(dir, fileName);

            await using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, ct);
            }

            var relPath = $"{datePath}/{fileName}".Replace("\\", "/");
            var publicUrl = $"{_opt.PublicBaseUrl.TrimEnd('/')}/{relPath}";

            return new StoredFileResult("Local", relPath, publicUrl);
        }

        private static string GuessExt(string contentType) => contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => ".bin"
        };
    }
}
