// Domain/Files/Entities/FileObject.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;

namespace Domain.Files.Entities
{
    public class FileObject : SoftDeletableEntity
    {
        public string StorageProvider { get; set; } = default!;
        public string StoragePath { get; set; } = default!;
        public string OriginalFileName { get; set; } = default!;

        public string? ContentType { get; set; }
        public long SizeBytes { get; set; }
        public string? Sha256 { get; set; } // char(64)

        public DateTime UploadedAt { get; set; }
        public Guid UploadedByUserId { get; set; }
        public User? UploadedByUser { get; set; }
    }
}
