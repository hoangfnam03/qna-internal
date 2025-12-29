namespace Application.Common.Options
{
    public class InviteOptions
    {
        public int ExpiresMinutes { get; set; } = 60 * 24; // 24h
        public string BaseUrl { get; set; } = "http://localhost:7100/invite"; // demo
    }
}
