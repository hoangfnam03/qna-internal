using Application.Announcements.DTOs;
using Application.Common.Interfaces;
using Domain.Communication.Entities;
using MediatR;

namespace Application.Announcements.Commands.CreateAnnouncement
{
    public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public CreateAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<Guid> Handle(CreateAnnouncementCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var title = request.Request.Title?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(title)) throw new InvalidOperationException("Title is required.");

            var entity = new Announcement
            {
                Title = title,
                Body = string.IsNullOrWhiteSpace(request.Request.Body) ? null : request.Request.Body.Trim(),
                IsPublished = false,
                PublishedAt = null,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _current.UserId.Value
            };

            _db.Announcements.Add(entity);
            await _db.SaveChangesAsync(ct);

            return entity.Id;
        }
    }
}
