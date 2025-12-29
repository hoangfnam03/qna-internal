// Infrastructure/Email/SmtpEmailSender.cs
using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;

        public SmtpEmailSender(IOptions<SmtpOptions> opt)
        {
            _opt = opt.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // Mailpit: không cần auth
            if (!string.IsNullOrWhiteSpace(_opt.Username))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_opt.Username, _opt.Password);
            }
            else
            {
                client.UseDefaultCredentials = false;
            }

            using var msg = new MailMessage
            {
                From = new MailAddress(_opt.FromEmail, _opt.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(toEmail);

            await Task.Run(() => client.Send(msg), ct);
        }
    }
}
