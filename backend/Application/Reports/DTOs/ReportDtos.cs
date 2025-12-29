using Domain.Moderation.Enums;

namespace Application.Reports.DTOs
{
    public record CreateReportRequest(
        ReportTargetType TargetType,
        Guid TargetId,
        ReportReasonCode? ReasonCode,
        string? ReasonText,
        string? EvidenceJson
    );

    public record ReportListItemDto(
        Guid Id,
        ReportStatus Status,
        ReportTargetType TargetType,
        Guid TargetId,
        ReportReasonCode? ReasonCode,
        DateTime CreatedAt,
        Guid ReporterUserId,
        string ReporterDisplayName
    );

    public record ReportDetailDto(
        Guid Id,
        ReportStatus Status,
        ReportTargetType TargetType,
        Guid TargetId,
        ReportReasonCode? ReasonCode,
        string? ReasonText,
        string? EvidenceJson,
        DateTime CreatedAt,
        Guid ReporterUserId,
        string ReporterDisplayName,
        DateTime? ReviewedAt,
        Guid? ReviewedByUserId,
        string? ReviewedByDisplayName,
        string? ResolutionNote
    );

    public record UpdateReportStatusRequest(
        ReportStatus Status,
        string? ResolutionNote
    );
}
