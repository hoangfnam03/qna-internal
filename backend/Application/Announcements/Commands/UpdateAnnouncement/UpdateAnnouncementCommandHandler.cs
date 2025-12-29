using Application.Announcements.DTOs;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Commands.UpdateAnnouncement
{
    public class UpdateAnnouncementCommandHandler : IRequestHandler<UpdateAnnouncementCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public UpdateAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(UpdateAnnouncementCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var a = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (a == null) throw new InvalidOperationException("Announcement not found.");

            var title = request.Request.Title?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(title)) throw new InvalidOperationException("Title is required.");

            a.Title = title;
            a.Body = string.IsNullOrWhiteSpace(request.Request.Body) ? null : request.Request.Body.Trim();
            a.UpdatedAt = DateTime.UtcNow;
            a.UpdatedByUserId = _current.UserId.Value;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
