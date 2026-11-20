using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Sms;

/// <summary>
/// Geliştirme ve test sağlayıcısı: mesajı infra.dev_sms_log'a yazar ve günlüğe basar (gövde, OTP dahil;
/// kodlama-dotnet.md yalnız bu sağlayıcıya ve Development'a izin verir). Üretimde seçilemez.
/// Gönderim işinin transaction'ında yazar: iş geri alınırsa kayıt da düşer.
/// </summary>
internal sealed partial class FakeSmsSender(InfraDbContext db, IClock clock, IIdGenerator ids, ILogger<FakeSmsSender> logger) : ISmsSender
{
    public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct)
    {
        var id = ids.New();
        db.DevSmsLog.Add(new DevSmsMessage
        {
            Id = id,
            PhoneE164 = message.PhoneE164,
            Sender = message.Sender,
            Body = message.Body,
            SentUtc = clock.GetCurrentInstant(),
        });
        await db.SaveChangesAsync(ct);
        LogSent(logger, PhoneNumber.TryParse(message.PhoneE164, out var phone) ? phone.Masked : "?", message.Sender, message.Body);
        return new SmsSendResult(Succeeded: true, ProviderMessageId: $"fake-{id}", FailureReason: null);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "[FakeSms] {Phone} <{Sender}> {Body}")]
    private static partial void LogSent(ILogger logger, string phone, string sender, string body);
}
