using Application.AI.Options;
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.AI.Services
{
    public class RagAnswerService
    {
        private readonly IAiServiceClient _ai;
        private readonly IVectorStore _vs;
        private readonly AiServiceOptions _opt;
        private readonly QdrantOptions _qopt;

        public RagAnswerService(
            IAiServiceClient ai,
            IVectorStore vs,
            IOptions<AiServiceOptions> opt,
            IOptions<QdrantOptions> qopt)
        {
            _ai = ai;
            _vs = vs;
            _opt = opt.Value;
            _qopt = qopt.Value;
        }

        public async Task<(string Answer, IReadOnlyList<VectorHit> Sources)> AnswerAsync(string question, CancellationToken ct)
        {
            question = (question ?? "").Trim();
            if (question.Length == 0) throw new InvalidOperationException("Question is empty.");

            var (dim, qv) = await _ai.EmbedAsync(new[] { question }, ct);
            await _vs.EnsureCollectionAsync(dim, ct);

            var hits = await _vs.SearchAsync(qv[0], _qopt.TopK, ct);

            // Build prompt with context
            var sb = new StringBuilder();
            sb.AppendLine("Bạn là trợ lý nội bộ. Trả lời ngắn gọn, đúng trọng tâm.");
            sb.AppendLine("Chỉ dựa trên phần NGỮ CẢNH dưới đây. Nếu không đủ thông tin thì nói 'Chưa có đủ thông tin trong tài liệu nội bộ'.");
            sb.AppendLine();
            sb.AppendLine("NGỮ CẢNH:");

            int maxChars = _opt.MaxContextChars <= 0 ? 12000 : _opt.MaxContextChars;
            int used = 0;

            foreach (var h in hits)
            {
                var block = $"[source:{h.SourceType}:{h.SourceId}:{h.ChunkIndex}] {h.Text}\n";
                if (used + block.Length > maxChars) break;
                sb.AppendLine(block);
                used += block.Length;
            }

            sb.AppendLine();
            sb.AppendLine("CÂU HỎI:");
            sb.AppendLine(question);
            sb.AppendLine();
            sb.AppendLine("TRẢ LỜI:");

            var answer = await _ai.GenerateAsync(sb.ToString(), ct);
            return (answer, hits);
        }
    }
}
