using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Omvila.Infrastructure.Email;

/// <summary>
/// Geliştirme ve test sağlayıcısı: iletiyi bellekte tutar (testler <see cref="Sent"/> ile okur) ve yalnız olay kodunu günlüğe yazar.
/// Üretimde seçilemez (FakeProviderOptionsValidator).
/// </summary>
public sealed partial class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    private readonly ConcurrentQueue<EmailMessage> _sent = new();

    /// <summary>Gönderilen iletiler (gönderim sırasıyla).</summary>
    public IReadOnlyCollection<EmailMessage> Sent => _sent.ToArray();

    public Task<EmailSendStatus> SendAsync(EmailMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);
        _sent.Enqueue(message);
        LogSent(logger, message.EventCode);
        return Task.FromResult(EmailSendStatus.Sent);
    }

    /// <summary>Bellekteki iletileri siler (test arası).</summary>
    public void Clear() => _sent.Clear();

    [LoggerMessage(Level = LogLevel.Information, Message = "[FakeEmail] {EventCode}")]
    private static partial void LogSent(ILogger logger, string eventCode);
}
