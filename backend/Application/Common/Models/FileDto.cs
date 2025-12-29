namespace Application.Common.Models
{
    public record FileDto(
        Guid Id,
        string OriginalFileName,
        string? ContentType,
        long SizeBytes,
        string Url
    );
}
