using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Files.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Files.Commands.UploadImage
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, FileDto>
    {
        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/png", "image/webp", "image/gif" };

        private const long MaxBytes = 5 * 1024 * 1024; // 5MB

        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly IFileStorage _storage;

        public UploadImageCommandHandler(IApplicationDbContext db, ICurrentUserService current, IFileStorage storage)
        {
            _db = db; _current = current; _storage = storage;
        }

        public async Task<FileDto> Handle(UploadImageCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var f = request.File;
            if (f == null || f.Length == 0) throw new InvalidOperationException("File is required.");
            if (f.Length > MaxBytes) throw new InvalidOperationException("Max 5MB.");
            if (string.IsNullOrWhiteSpace(f.ContentType) || !Allowed.Contains(f.ContentType))
                throw new InvalidOperationException("Only image/jpeg, image/png, image/webp, image/gif are allowed.");

            await using var stream = f.OpenReadStream();
            var stored = await _storage.SaveAsync(stream, f.FileName, f.ContentType, ct);

            var entity = new FileObject
            {
                StorageProvider = stored.Provider,
                StoragePath = stored.Path,
                OriginalFileName = f.FileName,
                ContentType = f.ContentType,
                SizeBytes = f.Length,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = _current.UserId.Value
            };

            _db.Files.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new FileDto(entity.Id, entity.OriginalFileName, entity.ContentType, entity.SizeBytes, stored.PublicUrl);
        }
    }
}
