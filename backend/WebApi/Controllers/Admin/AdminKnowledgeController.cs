using Application.Common.Interfaces;
using Application.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/knowledge")]
    [Authorize(Roles = "Admin")]
    public class AdminKnowledgeController : ControllerBase
    {
        private readonly IAiServiceClient _ai;
        private readonly IVectorStore _vs;

        public AdminKnowledgeController(IAiServiceClient ai, IVectorStore vs)
        {
            _ai = ai; _vs = vs;
        }

        public record IndexTextRequest(string SourceType, Guid SourceId, string Text);

        [HttpPost("index-text")]
        public async Task<IActionResult> IndexText(IndexTextRequest req, CancellationToken ct)
        {
            var chunks = TextChunker.Chunk(req.Text, chunkSize: 1200, overlap: 200);
            if (chunks.Count == 0) return Ok(new { indexed = 0 });

            var (dim, vectors) = await _ai.EmbedAsync(chunks, ct);
            await _vs.EnsureCollectionAsync(dim, ct);

            var points = chunks.Select((text, i) =>
            {
                var id = $"{req.SourceType}:{req.SourceId}:{i}";
                var payload = new
                {
                    sourceType = req.SourceType,
                    sourceId = req.SourceId,
                    chunkIndex = i,
                    text = text,
                    indexedAt = DateTime.UtcNow
                };
                return (Id: id, Vector: vectors[i], Payload: (object)payload);
            });

            await _vs.UpsertAsync(points, ct);
            return Ok(new { indexed = chunks.Count });
        }
    }
}
