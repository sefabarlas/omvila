using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Omvila.Infrastructure.Email;

/// <summary>E-posta yapılandırılmamış: gönderim yapılmaz.</summary>
internal sealed class DisabledEmailSender : IEmailSender
{
    public Task<EmailSendStatus> SendAsync(EmailMessage message, CancellationToken ct) => Task.FromResult(EmailSendStatus.Disabled);
}
