using System.Net.Http.Json;
using Application.AI.Options;
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.VectorStore
{
    public class QdrantVectorStore : IVectorStore
    {
        private readonly HttpClient _http;
        private readonly QdrantOptions _opt;

        public QdrantVectorStore(HttpClient http, IOptions<QdrantOptions> opt)
        {
            _http = http;
            _opt = opt.Value;
            _http.BaseAddress = new Uri(_opt.BaseUrl.TrimEnd('/'));
        }

        public async Task EnsureCollectionAsync(int vectorDim, CancellationToken ct)
        {
            // check exists
            var get = await _http.GetAsync($"/collections/{_opt.Collection}", ct);
            if (get.IsSuccessStatusCode) return;

            // create
            var body = new
            {
                vectors = new Dictionary<string, object>
                {
                    [_opt.VectorName] = new { size = vectorDim, distance = "Cosine" }
                }
            };

            var create = await _http.PutAsJsonAsync($"/collections/{_opt.Collection}", body, ct);
            create.EnsureSuccessStatusCode();
        }

        public async Task UpsertAsync(IEnumerable<(string Id, float[] Vector, object Payload)> points, CancellationToken ct)
        {
            var pts = points.Select(p => new
            {
                id = p.Id,
                vector = new Dictionary<string, float[]> { [_opt.VectorName] = p.Vector },
                payload = p.Payload
            }).ToList();

            var body = new { points = pts };
            var resp = await _http.PutAsJsonAsync($"/collections/{_opt.Collection}/points?wait=true", body, ct);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<IReadOnlyList<VectorHit>> SearchAsync(float[] queryVector, int topK, CancellationToken ct)
        {
            var body = new
            {
                vector = new Dictionary<string, float[]> { [_opt.VectorName] = queryVector },
                limit = topK,
                with_payload = true
            };

            var resp = await _http.PostAsJsonAsync($"/collections/{_opt.Collection}/points/search", body, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadFromJsonAsync<QdrantSearchResponse>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Qdrant search null response");

            return json.result.Select(r =>
            {
                var payload = r.payload ?? new Dictionary<string, object>();
                var text = payload.TryGetValue("text", out var t) ? Convert.ToString(t) ?? "" : "";
                var sourceType = payload.TryGetValue("sourceType", out var st) ? Convert.ToString(st) ?? "" : "";
                var sourceId = payload.TryGetValue("sourceId", out var sid) ? Guid.Parse(Convert.ToString(sid)!) : Guid.Empty;
                var chunkIndex = payload.TryGetValue("chunkIndex", out var ci) ? Convert.ToInt32(ci) : 0;

                return new VectorHit(r.id.ToString(), r.score, text, sourceType, sourceId, chunkIndex);
            }).ToList();
        }

        private sealed class QdrantSearchResponse
        {
            public List<QdrantHit> result { get; set; } = new();
        }

        private sealed class QdrantHit
        {
            public object id { get; set; } = default!;
            public float score { get; set; }
            public Dictionary<string, object>? payload { get; set; }
        }
    }
}
