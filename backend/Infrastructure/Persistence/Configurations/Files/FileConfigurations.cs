// Infrastructure/Persistence/Configurations/Files/FileConfigurations.cs
using Domain.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Files
{
    public class FileObjectConfiguration : IEntityTypeConfiguration<FileObject>
    {
        public void Configure(EntityTypeBuilder<FileObject> b)
        {
            b.ToTable("Files");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.StorageProvider).HasMaxLength(30).IsRequired();
            b.Property(x => x.StoragePath).HasMaxLength(1024).IsRequired();
            b.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
            b.Property(x => x.ContentType).HasMaxLength(128);

            b.Property(x => x.SizeBytes).HasColumnType("bigint").IsRequired();
            b.Property(x => x.Sha256).HasColumnType("char(64)");

            // unique filtered Sha256 (optional)
            b.HasIndex(x => x.Sha256)
             .IsUnique()
             .HasFilter("[Sha256] IS NOT NULL");

            b.Property(x => x.UploadedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.UploadedByUserId, x.UploadedAt }).IsDescending(false, true);

            b.HasOne(x => x.UploadedByUser)
             .WithMany()
             .HasForeignKey(x => x.UploadedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
