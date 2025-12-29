namespace Application.Common.Interfaces
{
    public record VectorHit(string Id, float Score, string Text, string SourceType, Guid SourceId, int ChunkIndex);

    public interface IVectorStore
    {
        Task EnsureCollectionAsync(int vectorDim, CancellationToken ct);
        Task UpsertAsync(IEnumerable<(string Id, float[] Vector, object Payload)> points, CancellationToken ct);
        Task<IReadOnlyList<VectorHit>> SearchAsync(float[] queryVector, int topK, CancellationToken ct);
    }
}
