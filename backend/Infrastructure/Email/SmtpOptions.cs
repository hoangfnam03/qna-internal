// Infrastructure/Email/SmtpOptions.cs
namespace Infrastructure.Email
{
    public class SmtpOptions
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;

        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string FromName { get; set; } = "QnA System";
        public string FromEmail { get; set; } = default!;
    }
}
