// Infrastructure/FileStorage/LocalFileStorageOptions.cs
namespace Infrastructure.FileStorage
{
    public class LocalFileStorageOptions
    {
        public string RootPath { get; set; } = "storage";
        public string PublicBaseUrl { get; set; } = "/storage";
    }
}
