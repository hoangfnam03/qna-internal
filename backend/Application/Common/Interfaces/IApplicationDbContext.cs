// Application/Common/Interfaces/IApplicationDbContext.cs
using Domain.Communication.Entities;
using Domain.Content.Entities;
using Domain.Content.Taxonomy;
using Domain.Files.Entities;
using Domain.Identity.Entities;
using Domain.Moderation.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<UserInvite> UserInvites { get; }
        DbSet<PasswordResetToken> PasswordResetTokens { get; }

        DbSet<Post> Posts { get; }
        DbSet<PostRevision> PostRevisions { get; }
        DbSet<PostAttachment> PostAttachments { get; }
        DbSet<PostTag> PostTags { get; }
        DbSet<PostVote> PostVotes { get; }

        DbSet<Category> Categories { get; }
        DbSet<Tag> Tags { get; }

        DbSet<Comment> Comments { get; }
        DbSet<CommentRevision> CommentRevisions { get; }
        DbSet<CommentAttachment> CommentAttachments { get; }
        DbSet<CommentVote> CommentVotes { get; }

        DbSet<FileObject> Files { get; }

        DbSet<Announcement> Announcements { get; }
        DbSet<AnnouncementAttachment> AnnouncementAttachments { get; }

        DbSet<Notification> Notifications { get; }
        DbSet<Report> Reports { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
