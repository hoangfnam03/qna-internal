using System.Net.Http.Json;
using Application.AI.Options;
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.AIClients
{
    public class AiServiceClient : IAiServiceClient
    {
        private readonly HttpClient _http;

        public AiServiceClient(HttpClient http, IOptions<AiServiceOptions> opt)
        {
            _http = http;
            _http.BaseAddress = new Uri(opt.Value.BaseUrl.TrimEnd('/'));
        }

        private sealed record InfoResp(string embed_model, int embed_dim);
        private sealed record EmbedReq(List<string> texts);
        private sealed record EmbedResp(string model, int dim, List<List<float>> vectors);
        private sealed record GenReq(string prompt, int max_new_tokens, float temperature, float top_p, bool do_sample);
        private sealed record GenResp(string model, string text);

        public async Task<(string EmbedModel, int EmbedDim)> GetInfoAsync(CancellationToken ct)
        {
            var r = await _http.GetFromJsonAsync<Dictionary<string, object>>("/info", ct)
                ?? throw new InvalidOperationException("AI service /info returned null.");

            // parse safely
            var dim = Convert.ToInt32(r["embed_dim"]);
            var model = Convert.ToString(r["embed_model"]) ?? "";
            return (model, dim);
        }

        public async Task<(int Dim, List<float[]> Vectors)> EmbedAsync(IReadOnlyList<string> texts, CancellationToken ct)
        {
            var resp = await _http.PostAsJsonAsync("/embed", new EmbedReq(texts.ToList()), ct);
            resp.EnsureSuccessStatusCode();

            var dto = await resp.Content.ReadFromJsonAsync<EmbedResp>(cancellationToken: ct)
                ?? throw new InvalidOperationException("AI service /embed returned null.");

            var vectors = dto.vectors.Select(v => v.Select(x => (float)x).ToArray()).ToList();
            return (dto.dim, vectors);
        }

        public async Task<string> GenerateAsync(string prompt, CancellationToken ct)
        {
            var req = new GenReq(prompt, max_new_tokens: 512, temperature: 0.2f, top_p: 0.9f, do_sample: true);

            var resp = await _http.PostAsJsonAsync("/generate", req, ct);
            resp.EnsureSuccessStatusCode();

            var dto = await resp.Content.ReadFromJsonAsync<GenResp>(cancellationToken: ct)
                ?? throw new InvalidOperationException("AI service /generate returned null.");

            return dto.text;
        }
    }
}
