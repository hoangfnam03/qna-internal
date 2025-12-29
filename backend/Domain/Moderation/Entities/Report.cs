// Domain/Moderation/Entities/Report.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;
using Domain.Moderation.Enums;

namespace Domain.Moderation.Entities
{
    public class Report : EntityBase
    {
        public Guid ReporterUserId { get; set; }
        public User? ReporterUser { get; set; }

        public ReportTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }

        public ReportReasonCode? ReasonCode { get; set; }
        public string? ReasonText { get; set; }

        public string? EvidenceJson { get; set; }

        public ReportStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public User? ReviewedByUser { get; set; }

        public string? ResolutionNote { get; set; }
    }
}
