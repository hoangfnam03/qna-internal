namespace Application.Announcements.DTOs
{
    public record AnnouncementAttachmentDto(
        Guid FileId,
        string OriginalFileName,
        string? ContentType,
        long SizeBytes,
        string Url
    );

    public record AnnouncementListItemDto(
        Guid Id,
        string Title,
        bool IsPublished,
        DateTime? PublishedAt,
        DateTime CreatedAt,
        string CreatedByDisplayName
    );

    public record AnnouncementDetailDto(
        Guid Id,
        string Title,
        string? Body,
        bool IsPublished,
        DateTime? PublishedAt,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string CreatedByDisplayName,
        List<AnnouncementAttachmentDto> Attachments
    );

    public record CreateAnnouncementRequest(string Title, string? Body);

    public record UpdateAnnouncementRequest(string Title, string? Body);
}
