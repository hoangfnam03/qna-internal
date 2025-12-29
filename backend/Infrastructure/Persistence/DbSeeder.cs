using Application.Common.Interfaces;
using Domain.Identity.Entities;
using Domain.Identity.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext db,
            IPasswordHasherService hasher,
            IConfiguration config,
            ILogger logger,
            CancellationToken ct = default)
        {
            // 1) Ensure DB migrated
            await db.Database.MigrateAsync(ct);

            // 2) Ensure roles
            var adminRoleName = "Admin";
            var userRoleName = "User";

            var roles = await db.Roles.ToListAsync(ct);
            if (!roles.Any(r => r.NormalizedName == adminRoleName.ToUpperInvariant()))
            {
                db.Roles.Add(new Role
                {
                    Name = adminRoleName,
                    NormalizedName = adminRoleName.ToUpperInvariant(),
                    Description = "System administrator"
                });
            }

            if (!roles.Any(r => r.NormalizedName == userRoleName.ToUpperInvariant()))
            {
                db.Roles.Add(new Role
                {
                    Name = userRoleName,
                    NormalizedName = userRoleName.ToUpperInvariant(),
                    Description = "Standard user"
                });
            }

            await db.SaveChangesAsync(ct);

            // reload roles to get IDs
            var adminRole = await db.Roles.FirstAsync(r => r.NormalizedName == adminRoleName.ToUpperInvariant(), ct);
            var userRole = await db.Roles.FirstAsync(r => r.NormalizedName == userRoleName.ToUpperInvariant(), ct);

            // 3) Seed admin user (configurable)
            var adminEmail = (config["Seed:AdminEmail"] ?? "admin@devask.local").Trim();
            var adminPassword = config["Seed:AdminPassword"] ?? "Admin123!";

            var normEmail = adminEmail.ToUpperInvariant();

            var adminUser = await db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normEmail, ct);

            if (adminUser == null)
            {
                var userName = (config["Seed:AdminUserName"] ?? "admin").Trim();

                adminUser = new User
                {
                    UserName = userName,
                    NormalizedUserName = userName.ToUpperInvariant(),
                    Email = adminEmail,
                    NormalizedEmail = normEmail,
                    EmailConfirmed = true,
                    DisplayName = config["Seed:AdminDisplayName"] ?? "Administrator",
                    Bio = null,
                    AvatarUrl = null,

                    // theo schema bạn đưa
                    Status = UserStatus.Active,
                    SuspendedUntil = null,
                    SuspensionReason = null,

                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = null
                };

                adminUser.PasswordHash = hasher.Hash(adminPassword);

                db.Users.Add(adminUser);
                await db.SaveChangesAsync(ct);

                logger.LogInformation("Seeded admin user {Email}", adminEmail);
            }

            // 4) Ensure admin has Admin role (and optionally User role)
            var hasAdminRole = await db.UserRoles.AnyAsync(
                ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id, ct);

            if (!hasAdminRole)
            {
                db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
            }

            var hasUserRole = await db.UserRoles.AnyAsync(
                ur => ur.UserId == adminUser.Id && ur.RoleId == userRole.Id, ct);

            if (!hasUserRole)
            {
                db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = userRole.Id });
            }

            await db.SaveChangesAsync(ct);

            logger.LogInformation("Seed completed. Admin: {Email}", adminEmail);
        }
    }
}
