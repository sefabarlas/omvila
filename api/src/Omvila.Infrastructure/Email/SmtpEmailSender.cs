using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Email;

/// <summary>
/// SMTP adaptörü (yerelde ve test sunucusunda Mailpit). .NET'in yerleşik istemcisi kullanılır: yeni bağımlılık gerekmez, Mailpit
/// düz SMTP'dir. Sağlayıcı (T-05) seçilince yalnız bu sınıf değişir.
/// </summary>
internal sealed partial class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task<EmailSendStatus> SendAsync(EmailMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);
        var settings = options.Value;
        using var mail = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromName, Encoding.UTF8),
            Subject = message.Subject,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            Body = message.TextBody,
            IsBodyHtml = false,
        };
        mail.To.Add(message.To);
        mail.Headers.Add("X-Omvila-Event", message.EventCode);
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.HtmlBody, Encoding.UTF8, MediaTypeNames.Text.Html));

        using var client = new SmtpClient(settings.Smtp.Host, settings.Smtp.Port) { EnableSsl = settings.Smtp.EnableSsl };
        if (!string.IsNullOrEmpty(settings.Smtp.UserName))
        {
            client.Credentials = new NetworkCredential(settings.Smtp.UserName, settings.Smtp.Password);
        }

        await client.SendMailAsync(mail, ct);
        LogSent(logger, message.EventCode);
        return EmailSendStatus.Sent;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Email {EventCode} sent via SMTP")]
    private static partial void LogSent(ILogger logger, string eventCode);
}
