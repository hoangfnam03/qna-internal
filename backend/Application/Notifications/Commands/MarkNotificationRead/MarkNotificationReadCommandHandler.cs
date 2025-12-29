using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Commands.MarkNotificationRead
{
    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public MarkNotificationReadCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;

            var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == request.NotificationId, ct);
            if (n == null) return true;

            if (n.RecipientUserId != uid) throw new UnauthorizedAccessException();

            if (!n.IsRead)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }

            return true;
        }
    }
}
