using Application.Common.Interfaces;
using Application.Reports.DTOs;
using Domain.Moderation.Entities;
using Domain.Moderation.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Reports.Commands.CreateReport
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public CreateReportCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<Guid> Handle(CreateReportCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;
            var r = request.Request;

            // validate target exists (tối thiểu: Post/Comment/User/Announcement)
            var targetExists = r.TargetType switch
            {
                ReportTargetType.Post => await _db.Posts.AsNoTracking().AnyAsync(x => x.Id == r.TargetId, ct),
                ReportTargetType.Comment => await _db.Comments.AsNoTracking().AnyAsync(x => x.Id == r.TargetId, ct),
                ReportTargetType.User => await _db.Users.AsNoTracking().AnyAsync(x => x.Id == r.TargetId, ct),
                ReportTargetType.Announcement => await _db.Announcements.AsNoTracking().AnyAsync(x => x.Id == r.TargetId, ct),
                _ => true
            };
            if (!targetExists) throw new InvalidOperationException("Target not found.");

            var reasonText = string.IsNullOrWhiteSpace(r.ReasonText) ? null : r.ReasonText.Trim();
            if (reasonText != null && reasonText.Length > 1000)
                throw new InvalidOperationException("ReasonText max 1000 chars.");

            var entity = new Report
            {
                ReporterUserId = uid,
                TargetType = r.TargetType,
                TargetId = r.TargetId,
                ReasonCode = r.ReasonCode,
                ReasonText = reasonText,
                EvidenceJson = string.IsNullOrWhiteSpace(r.EvidenceJson) ? null : r.EvidenceJson,
                Status = ReportStatus.Open,
                CreatedAt = DateTime.UtcNow,
                ReviewedAt = null,
                ReviewedByUserId = null,
                ResolutionNote = null
            };

            _db.Reports.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity.Id;
        }
    }
}
