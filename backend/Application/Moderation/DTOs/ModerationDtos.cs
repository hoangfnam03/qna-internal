using Domain.Moderation.Enums;

namespace Application.Moderation.DTOs
{
    public record DeleteTargetRequest(
        ReportTargetType TargetType,
        Guid TargetId,
        string? Note // ghi chú lý do xoá (optional)
    );
}
