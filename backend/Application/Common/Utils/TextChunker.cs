using System.Text;

namespace Application.Common.Utils
{
    public static class TextChunker
    {
        // chunk theo ký tự (đồ án đủ dùng). Sau này muốn token-based thì thay implementation, không đổi API.
        public static List<string> Chunk(string text, int chunkSize = 1200, int overlap = 200)
        {
            text ??= "";
            text = text.Trim();
            if (text.Length == 0) return new List<string>();

            if (chunkSize < 200) chunkSize = 200;
            if (overlap < 0) overlap = 0;
            if (overlap >= chunkSize) overlap = chunkSize / 3;

            var chunks = new List<string>();
            int start = 0;

            while (start < text.Length)
            {
                int len = Math.Min(chunkSize, text.Length - start);
                var piece = text.Substring(start, len).Trim();
                if (piece.Length > 0) chunks.Add(piece);

                if (start + len >= text.Length) break;
                start = start + len - overlap;
                if (start < 0) start = 0;
            }

            return chunks;
        }
    }
}
