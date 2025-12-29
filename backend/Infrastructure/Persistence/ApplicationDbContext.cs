// Infrastructure/Persistence/ApplicationDbContext.cs
using Application.Common.Interfaces;
using Domain.Communication.Entities;
using Domain.Content.Entities;
using Domain.Content.Taxonomy;
using Domain.Files.Entities;
using Domain.Identity.Entities;
using Domain.Moderation.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();


        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostRevision> PostRevisions => Set<PostRevision>();
        public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        public DbSet<PostVote> PostVotes => Set<PostVote>();

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();

        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<CommentRevision> CommentRevisions => Set<CommentRevision>();
        public DbSet<CommentAttachment> CommentAttachments => Set<CommentAttachment>();
        public DbSet<CommentVote> CommentVotes => Set<CommentVote>();

        public DbSet<FileObject> Files => Set<FileObject>();

        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<AnnouncementAttachment> AnnouncementAttachments => Set<AnnouncementAttachment>();

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Report> Reports => Set<Report>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.ApplySoftDeleteQueryFilters();
            base.OnModelCreating(modelBuilder);

        }
    }
}
