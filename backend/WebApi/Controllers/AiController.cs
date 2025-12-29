using Application.AI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/ai")]
    public class AiController : ControllerBase
    {
        private readonly RagAnswerService _rag;
        public AiController(RagAnswerService rag) => _rag = rag;

        public record AskRequest(string Question);
        public record AskResponse(string Answer, object[] Sources);

        [Authorize]
        [HttpPost("answer")]
        public async Task<IActionResult> Answer(AskRequest req, CancellationToken ct)
        {
            var (answer, sources) = await _rag.AnswerAsync(req.Question, ct);

            var src = sources.Select(s => new
            {
                s.Score,
                s.SourceType,
                s.SourceId,
                s.ChunkIndex,
                Preview = s.Text.Length <= 160 ? s.Text : s.Text.Substring(0, 160)
            }).ToArray();

            return Ok(new AskResponse(answer, src));
        }
    }
}
