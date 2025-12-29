// Infrastructure/Persistence/Configurations/Content/TaxonomyConfigurations.cs
using Domain.Content.Taxonomy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Content
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> b)
        {
            b.ToTable("Categories");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Name).HasMaxLength(120).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(128).IsRequired();
            b.HasIndex(x => x.Slug).IsUnique();

            b.Property(x => x.SortOrder).HasDefaultValue(0);
            b.Property(x => x.IsHidden).HasDefaultValue(false);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();

            b.HasOne(x => x.Parent)
             .WithMany(x => x.Children)
             .HasForeignKey(x => x.ParentId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> b)
        {
            b.ToTable("Tags");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Name).HasMaxLength(64).IsRequired();
            b.Property(x => x.Slug).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.Slug).IsUnique();

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();
        }
    }
}
