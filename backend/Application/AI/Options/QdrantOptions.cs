namespace Application.AI.Options
{
    public class QdrantOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:6333";
        public string Collection { get; set; } = "kb";
        public string VectorName { get; set; } = "dense_v1";
        public int TopK { get; set; } = 8;
    }
}
