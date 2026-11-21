using Microsoft.Extensions.Logging;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Push;

/// <summary>
/// Geliştirme ve test sağlayıcısı: kabul eder ve yalnız başlığı günlüğe yazar (jeton ve metin yazılmaz: metinde müşteri
/// adı olabilir). Üretimde seçilemez.
/// </summary>
internal sealed partial class FakePushSender(IIdGenerator ids, ILogger<FakePushSender> logger) : IPushSender
{
    public Task<PushSendResult> SendAsync(PushMessage message, CancellationToken ct)
    {
        LogSent(logger, message.Title);
        return Task.FromResult(new PushSendResult(PushSendStatus.Sent, $"fake-push-{ids.New()}"));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "[FakePush] {Title}")]
    private static partial void LogSent(ILogger logger, string title);
}
