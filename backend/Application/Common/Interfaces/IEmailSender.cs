// Application/Common/Interfaces/IEmailSender.cs
namespace Application.Common.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }
}
