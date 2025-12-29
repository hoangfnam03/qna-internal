// Infrastructure/Persistence/Configurations/Common/EntityConventions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Common
{
    internal static class EntityConventions
    {
        public static void ConfigureGuidPk<T>(this EntityTypeBuilder<T> b) where T : class
        {
            b.Property("Id")
             .HasDefaultValueSql("newsequentialid()");
        }

        public static PropertyBuilder<DateTime> AsDateTime2(this PropertyBuilder<DateTime> p)
            => p.HasColumnType("datetime2");

        public static PropertyBuilder<DateTime?> AsDateTime2(this PropertyBuilder<DateTime?> p)
            => p.HasColumnType("datetime2");
    }
}
