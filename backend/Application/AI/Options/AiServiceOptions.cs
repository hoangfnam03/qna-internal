namespace Application.AI.Options
{
    public class AiServiceOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:8000";
        public int MaxContextChars { get; set; } = 12000;
    }
}
