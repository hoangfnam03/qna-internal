using System.Linq.Expressions;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.Persistence
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null) continue;

                if (!typeof(ISoftDeleteEntity).IsAssignableFrom(clrType)) continue;

                // e => !e.IsDeleted
                var param = Expression.Parameter(clrType, "e");
                var prop = Expression.Property(param, nameof(ISoftDeleteEntity.IsDeleted));
                var body = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(body, param);

                entityType.SetQueryFilter(lambda);
            }

            return modelBuilder;
        }
    }
}
