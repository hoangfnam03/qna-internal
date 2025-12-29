namespace Application.Attachments.DTOs
{
    public record AddAttachmentRequest(
        Guid FileId,
        string? Caption,
        string? DisplayText,
        int SortOrder = 0
    );
}
