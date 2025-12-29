namespace Application.Common.Interfaces
{
    public record StoredFileResult(string Provider, string Path, string PublicUrl);

    public interface IFileStorage
    {
        Task<StoredFileResult> SaveAsync(
            Stream content,
            string originalFileName,
            string contentType,
            CancellationToken ct);
    }
}
