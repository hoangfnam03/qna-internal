namespace Application.Common.Interfaces
{
    public interface IAiServiceClient
    {
        Task<(int Dim, List<float[]> Vectors)> EmbedAsync(IReadOnlyList<string> texts, CancellationToken ct);
        Task<string> GenerateAsync(string prompt, CancellationToken ct);
        Task<(string EmbedModel, int EmbedDim)> GetInfoAsync(CancellationToken ct);
    }
}
