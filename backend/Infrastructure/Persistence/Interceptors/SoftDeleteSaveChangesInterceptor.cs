using Application.Common.Interfaces;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors
{
    public class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _current;

        public SoftDeleteSaveChangesInterceptor(ICurrentUserService current)
        {
            _current = current;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var ctx = eventData.Context;
            if (ctx == null) return base.SavingChanges(eventData, result);

            foreach (var entry in ctx.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Deleted) continue;

                if (entry.Entity is ISoftDeleteEntity soft)
                {
                    entry.State = EntityState.Modified;
                    soft.IsDeleted = true;
                    soft.DeletedAt = DateTime.UtcNow;
                    soft.DeletedByUserId = _current.UserId;
                }
            }

            return base.SavingChanges(eventData, result);
        }
    }
}
