// Infrastructure/Persistence/Configurations/Communication/CommunicationConfigurations.cs
using Domain.Communication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Communication
{
    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> b)
        {
            b.ToTable("Announcements");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Body);

            b.Property(x => x.IsPublished).HasDefaultValue(false);
            b.Property(x => x.PublishedAt).AsDateTime2();

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();

            b.HasIndex(x => new { x.IsPublished, x.PublishedAt }).IsDescending(false, true);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.UpdatedByUser)
             .WithMany()
             .HasForeignKey(x => x.UpdatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Attachments)
             .WithOne(x => x.Announcement)
             .HasForeignKey(x => x.AnnouncementId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AnnouncementAttachmentConfiguration : IEntityTypeConfiguration<AnnouncementAttachment>
    {
        public void Configure(EntityTypeBuilder<AnnouncementAttachment> b)
        {
            b.ToTable("AnnouncementAttachments");
            b.HasKey(x => new { x.AnnouncementId, x.FileId });

            b.HasOne(x => x.Announcement)
             .WithMany(x => x.Attachments)
             .HasForeignKey(x => x.AnnouncementId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.File)
             .WithMany()
             .HasForeignKey(x => x.FileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(x => !x.Announcement.IsDeleted);
        }
    }

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> b)
        {
            b.ToTable("Notifications");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Type).HasColumnType("tinyint").IsRequired();
            b.Property(x => x.EntityType).HasColumnType("tinyint");
            b.Property(x => x.DataJson);

            b.Property(x => x.IsRead).HasDefaultValue(false);
            b.Property(x => x.ReadAt).AsDateTime2();
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.RecipientUserId, x.IsRead, x.CreatedAt })
             .IsDescending(false, false, true);

            b.HasOne(x => x.RecipientUser)
             .WithMany()
             .HasForeignKey(x => x.RecipientUserId)
             .OnDelete(DeleteBehavior.Restrict);

            // Actor nullable -> SetNull khi user bị xoá cứng (nếu có)
            b.HasOne(x => x.ActorUser)
             .WithMany()
             .HasForeignKey(x => x.ActorUserId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasQueryFilter(n => !n.RecipientUser.IsDeleted);
        }
    }
}
