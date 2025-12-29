using System.Text.Json;
using Application.Common.Interfaces;
using Application.Reports.DTOs;
using Domain.Communication.Entities;
using Domain.Communication.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Reports.Commands.AdminUpdateReportStatus
{
    public class AdminUpdateReportStatusCommandHandler : IRequestHandler<AdminUpdateReportStatusCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminUpdateReportStatusCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(AdminUpdateReportStatusCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var adminId = _current.UserId.Value;

            var r = await _db.Reports.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (r == null) throw new InvalidOperationException("Report not found.");

            r.Status = request.Request.Status;
            r.ReviewedAt = DateTime.UtcNow;
            r.ReviewedByUserId = adminId;
            r.ResolutionNote = string.IsNullOrWhiteSpace(request.Request.ResolutionNote)
                ? null
                : request.Request.ResolutionNote.Trim();

            // notify reporter (nếu reporter != admin)
            if (r.ReporterUserId != adminId)
            {
                var payload = JsonSerializer.Serialize(new
                {
                    reportId = r.Id,
                    status = r.Status.ToString(),
                    targetType = r.TargetType.ToString(),
                    targetId = r.TargetId,
                    note = r.ResolutionNote
                });

                _db.Notifications.Add(new Notification
                {
                    RecipientUserId = r.ReporterUserId,
                    ActorUserId = adminId,
                    Type = NotificationType.ReportStatusChanged,
                    EntityType = EntityType.Report,
                    EntityId = r.Id,
                    DataJson = payload,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
