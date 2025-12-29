// Infrastructure/Persistence/Configurations/Identity/IdentityConfigurations.cs
using Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Configurations.Common;

namespace Infrastructure.Persistence.Configurations.Identity
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.UserName).HasMaxLength(64).IsRequired();
            b.Property(x => x.NormalizedUserName).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.NormalizedUserName).IsUnique();

            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.NormalizedEmail).HasMaxLength(256);

            // optional unique filtered index (SQL Server)
            b.HasIndex(x => x.NormalizedEmail)
             .IsUnique()
             .HasFilter("[NormalizedEmail] IS NOT NULL");

            b.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Bio).HasMaxLength(1000);
            b.Property(x => x.AvatarUrl).HasMaxLength(1024);

            b.Property(x => x.SecurityStamp).HasMaxLength(64);
            b.Property(x => x.Status).HasColumnType("tinyint").IsRequired();

            b.Property(x => x.SuspensionReason).HasMaxLength(500);

            // Audit + SoftDelete
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.UpdatedAt).AsDateTime2();

            // Relations
            b.HasMany(x => x.UserRoles)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.RefreshTokens)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.UserInvites)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> b)
        {
            b.ToTable("Roles");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.Name).HasMaxLength(64).IsRequired();
            b.Property(x => x.NormalizedName).HasMaxLength(64).IsRequired();
            b.HasIndex(x => x.NormalizedName).IsUnique();

            b.Property(x => x.Description).HasMaxLength(256);

            b.HasMany(x => x.UserRoles)
             .WithOne(x => x.Role)
             .HasForeignKey(x => x.RoleId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> b)
        {
            b.ToTable("UserRoles");
            b.HasKey(x => new { x.UserId, x.RoleId });

            b.HasOne(x => x.User)
             .WithMany(x => x.UserRoles)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Role)
             .WithMany(x => x.UserRoles)
             .HasForeignKey(x => x.RoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(x => !x.User.IsDeleted);
        }
    }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.ToTable("RefreshTokens");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.TokenHash).HasColumnType("char(64)").IsRequired();
            b.Property(x => x.ReplacedByTokenHash).HasColumnType("char(64)");

            b.Property(x => x.ExpiresAt).AsDateTime2().IsRequired();
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();
            b.Property(x => x.RevokedAt).AsDateTime2();

            b.Property(x => x.Device).HasMaxLength(120);
            b.Property(x => x.IpAddress).HasMaxLength(64);

            b.HasIndex(x => new { x.UserId, x.ExpiresAt });

            b.HasOne(x => x.User)
             .WithMany(x => x.RefreshTokens)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(x => !x.User.IsDeleted);
        }
    }

    public class UserInviteConfiguration : IEntityTypeConfiguration<UserInvite>
    {
        public void Configure(EntityTypeBuilder<UserInvite> b)
        {
            b.ToTable("UserInvites");
            b.HasKey(x => x.Id);
            b.ConfigureGuidPk();

            b.Property(x => x.TokenHash).HasColumnType("char(64)").IsRequired();
            b.HasIndex(x => x.TokenHash).IsUnique();

            b.Property(x => x.ExpiresAt).AsDateTime2().IsRequired();
            b.Property(x => x.UsedAt).AsDateTime2();
            b.Property(x => x.CreatedAt).AsDateTime2().IsRequired();

            b.Property(x => x.SentToEmail).HasMaxLength(256);

            b.HasIndex(x => new { x.UserId, x.ExpiresAt });

            b.HasOne(x => x.User)
             .WithMany(x => x.UserInvites)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasQueryFilter(x => !x.User.IsDeleted);
        }
    }
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> b)
        {
            b.ToTable("PasswordResetTokens");

            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            b.Property(x => x.TokenHash).HasColumnType("char(64)").IsRequired();
            b.HasIndex(x => x.TokenHash).IsUnique();

            b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
            b.Property(x => x.ExpiresAt).HasColumnType("datetime2").IsRequired();
            b.Property(x => x.UsedAt).HasColumnType("datetime2");

            b.Property(x => x.SentToEmail).HasMaxLength(256);

            b.HasIndex(x => new { x.UserId, x.ExpiresAt });

            b.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasQueryFilter(x => !x.User.IsDeleted);
        }
    }
}
