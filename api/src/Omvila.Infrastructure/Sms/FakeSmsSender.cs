using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Sms;

/// <summary>
/// Geliştirme ve test sağlayıcısı: mesajı infra.dev_sms_log'a yazar (test sunucusunda /dev/sms sayfasından okunur, T-76).
/// Gövde (OTP dahil) ve maskeli telefon yalnız Development'ta günlüğe basılır (kodlama-dotnet.md); diğer ortamlarda günlükte
/// yalnız kayıt kimliği vardır (barındırıcısız test servis koleksiyonunda da). Üretimde seçilemez.
/// Gönderim işinin transaction'ında yazar: iş geri alınırsa kayıt da düşer.
/// </summary>
internal sealed partial class FakeSmsSender(InfraDbContext db, IClock clock, IIdGenerator ids, ILogger<FakeSmsSender> logger, IHostEnvironment? environment = null) : ISmsSender
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
        if (environment?.AllowsSmsContentInLogs() == true)
        {
            LogSent(logger, PhoneNumber.TryParse(message.PhoneE164, out var phone) ? phone.Masked : "?", message.Sender, message.Body);
        }
        else
        {
            LogStored(logger, id);
        }

        return new SmsSendResult(Succeeded: true, ProviderMessageId: $"fake-{id}", FailureReason: null);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "[FakeSms] {Phone} <{Sender}> {Body}")]
    private static partial void LogSent(ILogger logger, string phone, string sender, string body);

    [LoggerMessage(Level = LogLevel.Information, Message = "[FakeSms] infra.dev_sms_log kaydı {Id}")]
    private static partial void LogStored(ILogger logger, Guid id);
}
