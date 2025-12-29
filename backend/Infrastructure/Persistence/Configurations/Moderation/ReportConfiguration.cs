// Infrastructure/Persistence/Configurations/Moderation/ReportConfiguration.cs
using Domain.Moderation.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Moderation
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> b)
        {
            b.ToTable("Reports");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.TargetType).HasColumnType("tinyint").IsRequired();
            b.Property(x => x.ReasonCode).HasColumnType("tinyint");
            b.Property(x => x.Status).HasColumnType("tinyint").IsRequired();

            b.Property(x => x.ReasonText).HasMaxLength(1000);
            b.Property(x => x.EvidenceJson);
            b.Property(x => x.ResolutionNote).HasMaxLength(1000);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.ReviewedAt).AsDateTime2();

            b.HasIndex(x => new { x.Status, x.CreatedAt }).IsDescending(false, true);
            b.HasIndex(x => new { x.TargetType, x.TargetId, x.Status });

            b.HasOne(x => x.ReporterUser)
             .WithMany()
             .HasForeignKey(x => x.ReporterUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ReviewedByUser)
             .WithMany()
             .HasForeignKey(x => x.ReviewedByUserId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasQueryFilter(x => !x.ReporterUser.IsDeleted);
        }
    }
}
