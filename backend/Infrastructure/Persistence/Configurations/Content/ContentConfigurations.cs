// Infrastructure/Persistence/Configurations/Content/ContentConfigurations.cs
using Domain.Content.Entities;
using Domain.Content.Taxonomy;
using Domain.Files.Entities;
using Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Content
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> b)
        {
            b.ToTable("Posts");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
            b.Property(x => x.Body).IsRequired();
            b.Property(x => x.IsDeleted).HasDefaultValue(false);    

            b.Property(x => x.Score).HasDefaultValue(0);
            b.Property(x => x.CommentCount).HasDefaultValue(0);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();

            // indexes (EF Core 7+ hỗ trợ IsDescending)
            b.HasIndex(x => x.CreatedAt).IsDescending();
            b.HasIndex(x => new { x.CreatedByUserId, x.CreatedAt }).IsDescending(false, true);
            b.HasIndex(x => new { x.CategoryId, x.CreatedAt }).IsDescending(false, true);

            // FK
            b.HasOne(x => x.Category)
             .WithMany()
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.UpdatedByUser)
             .WithMany()
             .HasForeignKey(x => x.UpdatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Revisions)
             .WithOne(x => x.Post)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Attachments)
             .WithOne(x => x.Post)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.PostTags)
             .WithOne(x => x.Post)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Votes)
             .WithOne(x => x.Post)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PostRevisionConfiguration : IEntityTypeConfiguration<PostRevision>
    {
        public void Configure(EntityTypeBuilder<PostRevision> b)
        {
            b.ToTable("PostRevisions");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.BeforeTitle).HasMaxLength(200);
            b.Property(x => x.AfterTitle).HasMaxLength(200);
            b.Property(x => x.Summary).HasMaxLength(255);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.PostId, x.CreatedAt }).IsDescending(false, true);

            b.HasOne(x => x.Post)
             .WithMany(x => x.Revisions)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.EditedByUser)
             .WithMany()
             .HasForeignKey(x => x.EditedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(r => !r.Post.IsDeleted);
        }
    }

    public class PostAttachmentConfiguration : IEntityTypeConfiguration<PostAttachment>
    {
        public void Configure(EntityTypeBuilder<PostAttachment> b)
        {
            b.ToTable("PostAttachments");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Caption).HasMaxLength(255);
            b.Property(x => x.DisplayText).HasMaxLength(255);
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.PostId, x.SortOrder });

            b.HasOne(x => x.Post)
             .WithMany(x => x.Attachments)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.File)
             .WithMany()
             .HasForeignKey(x => x.FileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(a => !a.Post.IsDeleted);
        }
    }

    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> b)
        {
            b.ToTable("PostTags");
            b.HasKey(x => new { x.PostId, x.TagId });

            b.HasIndex(x => x.TagId);

            b.HasOne(x => x.Post)
             .WithMany(x => x.PostTags)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Tag)
             .WithMany(x => x.PostTags)
             .HasForeignKey(x => x.TagId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(pt => !pt.Post.IsDeleted);
        }
    }

    public class PostVoteConfiguration : IEntityTypeConfiguration<PostVote>
    {
        public void Configure(EntityTypeBuilder<PostVote> b)
        {
            b.ToTable("PostVotes", t =>
            {
                t.HasCheckConstraint("CK_PostVotes_Value", "[Value] IN (-1, 1)");
            });

            b.HasKey(x => new { x.PostId, x.UserId });

            b.Property(x => x.Value).HasColumnType("smallint").IsRequired();
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.UserId, x.CreatedAt }).IsDescending(false, true);

            b.HasOne(x => x.Post)
             .WithMany(x => x.Votes)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(v => !v.Post.IsDeleted);
        }
    }

    // ---------- COMMENTS ----------

    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> b)
        {
            b.ToTable("Comments");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Body).IsRequired();
            b.Property(x => x.Score).HasDefaultValue(0);
            b.Property(x => x.IsDeleted).HasDefaultValue(false);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();

            b.HasIndex(x => new { x.PostId, x.CreatedAt });
            b.HasIndex(x => new { x.CreatedByUserId, x.CreatedAt }).IsDescending(false, true);

            b.HasOne(x => x.Post)
             .WithMany(x => x.Comments)
             .HasForeignKey(x => x.PostId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ParentComment)
             .WithMany(x => x.Replies)
             .HasForeignKey(x => x.ParentCommentId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.UpdatedByUser)
             .WithMany()
             .HasForeignKey(x => x.UpdatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Revisions)
             .WithOne(x => x.Comment)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Attachments)
             .WithOne(x => x.Comment)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Votes)
             .WithOne(x => x.Comment)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CommentVoteConfiguration : IEntityTypeConfiguration<CommentVote>
    {
        public void Configure(EntityTypeBuilder<CommentVote> b)
        {
            b.ToTable("CommentVotes", t =>
            {
                t.HasCheckConstraint("CK_CommentVotes_Value", "[Value] IN (-1, 1)");
            });

            b.HasKey(x => new { x.CommentId, x.UserId });

            b.Property(x => x.Value).HasColumnType("smallint").IsRequired();
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.UserId, x.CreatedAt }).IsDescending(false, true);

            b.HasOne(x => x.Comment)
             .WithMany(x => x.Votes)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(v => !v.Comment.IsDeleted);
        }
    }

    public class CommentRevisionConfiguration : IEntityTypeConfiguration<CommentRevision>
    {
        public void Configure(EntityTypeBuilder<CommentRevision> b)
        {
            b.ToTable("CommentRevisions");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Summary).HasMaxLength(255);
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.CommentId, x.CreatedAt }).IsDescending(false, true);

            b.HasOne(x => x.Comment)
             .WithMany(x => x.Revisions)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.EditedByUser)
             .WithMany()
             .HasForeignKey(x => x.EditedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(r => !r.Comment.IsDeleted);
        }
    }

    public class CommentAttachmentConfiguration : IEntityTypeConfiguration<CommentAttachment>
    {
        public void Configure(EntityTypeBuilder<CommentAttachment> b)
        {
            b.ToTable("CommentAttachments");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Caption).HasMaxLength(255);
            b.Property(x => x.DisplayText).HasMaxLength(255);
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.HasIndex(x => new { x.CommentId, x.SortOrder });

            b.HasOne(x => x.Comment)
             .WithMany(x => x.Attachments)
             .HasForeignKey(x => x.CommentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.File)
             .WithMany()
             .HasForeignKey(x => x.FileId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(a => !a.Comment.IsDeleted);
        }
    }
}
