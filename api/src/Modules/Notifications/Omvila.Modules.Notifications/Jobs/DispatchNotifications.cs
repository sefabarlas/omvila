using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Push;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Templates;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Jobs;

/// <summary>
/// DispatchNotifications (docs/mimari/mimari.md § Arka plan işleri; dilim-03 kriter 7, 10, 11): zamanı gelmiş bekleyen
/// bildirimleri (ScheduledUtc ≤ şimdi; sessiz saate kaydırılmış hatırlatma kaydırılmış saatinden önce gitmez) şablonla
/// oluşturup gönderir. Platform kapsamlı iş transaction'ında çalışır (JobRunner); satırlar FOR UPDATE SKIP LOCKED ile
/// alınır, marka defteri <see cref="CreditLedger.LockBalanceAsync"/> ile transaction sonuna kadar kilitlenir: kontör
/// düşümü ve Sent işareti aynı commit'tedir, iki örnek aynı bakiyeyi harcayamaz.
/// Kontör: salon mesajı parça sayısı kadar düşer (bildirimler.md § Kontör); yetmezse Skipped/insufficient_credits ve
/// marka başına bir <see cref="SmsCreditsInsufficient"/> olayı. Giriş kodu (ve markasız satır) OMVILA'dan karşılanır,
/// gönderen her zaman varsayılandır. Gönderen: onaylı ad, yoksa varsayılan (OMVILA).
/// Kanal planı (bildirimler.md § Kanal planı): planı Push ile başlayan satır alıcının aktif cihazlarına gönderilir; biri
/// kabul ederse Sent/Push (kontör düşmez). Cihaz yok, push kapalı veya başarısızsa müşteri mesajı aynı çalışmada SMS'e
/// düşer; yalnız push olan işletme satırı cihaz yoksa Skipped/no_device, geçici hatada geri çekilmeyle tekrar. Geçersiz
/// jetonlu cihaz pasifleşir.
/// Gönderim en az bir kez: commit düşerse mesaj yeniden gönderilebilir.
/// </summary>
internal sealed partial class DispatchNotifications(
    NotificationsDbContext db,
    ISmsSender sms,
    IPushSender push,
    IOutbox outbox,
    IClock clock,
    IIdGenerator ids,
    IOptions<NotificationsOptions> options,
    IOptions<SmsOptions> smsOptions,
    ILogger<DispatchNotifications> logger) : IPeriodicJob
{
    public const string InsufficientCredits = "insufficient_credits";

    private const string RenderFailed = "render_failed";

    private const string ProviderError = "provider_error";

    private const string NoDevice = "no_device";

    public string Key => "DispatchNotifications";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var batchSize = options.Value.BatchSize;
        var due = await db.Outbox
            .FromSql($"SELECT * FROM notifications.notification_outbox WHERE status = 'Pending' AND scheduled_utc <= {now} ORDER BY scheduled_utc, id LIMIT {batchSize} FOR UPDATE SKIP LOCKED")
            .ToListAsync(ct);
        if (due.Count == 0)
        {
            return;
        }

        var brandIds = due.Where(m => m.BrandId is not null).Select(m => m.BrandId!.Value).Distinct().ToList();
        var senders = await db.SmsSenders.AsNoTracking().Where(s => brandIds.Contains(s.BrandId)).ToDictionaryAsync(s => s.BrandId, ct);
        var run = new RunState();

        foreach (var message in due)
        {
            await DispatchAsync(message, senders, run, now, ct);
        }

        await db.SaveChangesAsync(ct);

        foreach (var (brandId, count) in run.Skipped)
        {
            await outbox.EnqueueAsync(new SmsCreditsInsufficient(brandId, count, run.Balances[brandId]), ct);
        }

        foreach (var brandId in run.CrossedLow)
        {
            await outbox.EnqueueAsync(new SmsCreditsLow(brandId, run.Balances[brandId], PlatformDefaults.LowCreditThreshold), ct);
        }
    }

    private async Task DispatchAsync(
        NotificationMessage message, Dictionary<Guid, BrandSmsSender> senders, RunState run, Instant now, CancellationToken ct)
    {
        if (message.PushFirst)
        {
            var pushed = await TryPushAsync(message, now, ct);
            if (pushed is null || message.Status != NotificationStatus.Pending)
            {
                return;
            }

            if (!message.HasSmsFallback)
            {
                // İşletme push'u SMS'e düşmez (bildirimler.md § İşletme olayları).
                if (pushed == PushSendStatus.Failed)
                {
                    message.MarkAttemptFailed(ProviderError, now, Backoff, options.Value.MaxAttempts);
                    LogFailed(logger, message.Id, message.EventCode);
                }
                else
                {
                    message.MarkSkipped(NoDevice);
                }

                return;
            }
        }

        await SendSmsAsync(message, senders, run, now, ct);
    }

    /// <summary>
    /// Alıcının aktif cihazlarına push. Biri kabul ederse satır Sent/Push ve null döner; aksi en iyi sonuç (Failed: en az
    /// bir geçici hata; Disabled/InvalidToken: ulaşılacak cihaz yok).
    /// </summary>
    private async Task<PushSendStatus?> TryPushAsync(NotificationMessage message, Instant now, CancellationToken ct)
    {
        string title;
        string body;
        try
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Payload) ?? [];
            if (!PushTemplates.TryRender(message.EventCode, values, message.TemplateVariant, out title, out body))
            {
                return PushSendStatus.Disabled;
            }
        }
        catch (Exception exception) when (exception is InvalidOperationException or JsonException)
        {
            message.MarkAttemptFailed(RenderFailed, now, Backoff, options.Value.MaxAttempts, permanent: true);
            LogFailed(logger, message.Id, message.EventCode);
            return null;
        }

        // İşletme cihazı marka bağlamında kayıtlıdır: yalnız o markanın cihazları (kullanıcı başka markada da üye olabilir).
        var userOwner = string.Equals(message.RecipientType, Device.UserOwner, StringComparison.Ordinal);
        var devices = await db.Devices
            .Where(d => d.IsActive && d.OwnerType == message.RecipientType && d.OwnerId == message.RecipientId
                && (!userOwner || d.BrandId == message.BrandId))
            .OrderBy(d => d.Id)
            .ToListAsync(ct);
        var data = new Dictionary<string, string>(StringComparer.Ordinal) { ["eventCode"] = message.EventCode, ["notificationId"] = message.Id.ToString() };
        if (message.ReferenceType is { } referenceType && message.ReferenceId is { } referenceId)
        {
            data["referenceType"] = referenceType;
            data["referenceId"] = referenceId.ToString();
        }

        var outcome = PushSendStatus.Disabled;
        string? providerMessageId = null;
        foreach (var device in devices)
        {
            var result = await PushAsync(new PushMessage(device.PushToken, title, body, data), ct);
            switch (result.Status)
            {
                case PushSendStatus.Sent:
                    providerMessageId ??= result.ProviderMessageId;
                    break;
                case PushSendStatus.InvalidToken:
                    device.Deactivate();
                    break;
                case PushSendStatus.Failed:
                    outcome = PushSendStatus.Failed;
                    break;
                case PushSendStatus.Disabled:
                    break;
            }
        }

        if (providerMessageId is not null)
        {
            message.MarkPushed(body, providerMessageId);
            return null;
        }

        return outcome;
    }

    private async Task<PushSendResult> PushAsync(PushMessage message, CancellationToken ct)
    {
        try
        {
            return await push.SendAsync(message, ct);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogProviderException(logger, exception.GetType().Name);
            return new PushSendResult(PushSendStatus.Failed, FailureReason: ProviderError);
        }
    }

    private async Task SendSmsAsync(
        NotificationMessage message, Dictionary<Guid, BrandSmsSender> senders, RunState run, Instant now, CancellationToken ct)
    {
        string body;
        try
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Payload) ?? [];
            body = SmsTemplates.Render(message.EventCode, values, message.TemplateVariant);
        }
        catch (Exception exception) when (exception is InvalidOperationException or JsonException)
        {
            // Bozuk satır kuyruğu tıkamaz: kalıcı hata.
            message.MarkAttemptFailed(RenderFailed, now, Backoff, options.Value.MaxAttempts, permanent: true);
            LogFailed(logger, message.Id, message.EventCode);
            return;
        }

        // Kural: bildirimler.md § Gönderen ve § Kontör — giriş kodu her zaman OMVILA adıyla ve OMVILA hesabından.
        var payingBrand = string.Equals(message.EventCode, NotificationEvents.OtpCode, StringComparison.Ordinal) ? null : message.BrandId;
        var defaultSender = smsOptions.Value.DefaultSender;
        var sender = payingBrand is { } b ? BrandSmsSender.EffectiveName(senders.GetValueOrDefault(b), defaultSender) : defaultSender;
        var credits = 0;
        var balance = 0;
        if (payingBrand is { } brandId)
        {
            credits = SmsEncoding.Measure(body).Segments;
            if (!run.Balances.TryGetValue(brandId, out balance))
            {
                balance = await CreditLedger.LockBalanceAsync(db, brandId, ct);
                run.Balances[brandId] = balance;
            }

            // Kural: alan-modeli.md § CreditTransaction — bakiye negatif olamaz; yetersizse gönderilmez.
            if (balance < credits)
            {
                message.MarkSkipped(InsufficientCredits);
                run.Skipped[brandId] = run.Skipped.GetValueOrDefault(brandId) + 1;
                return;
            }
        }

        var result = await SendAsync(new SmsMessage(message.RecipientPhone!, sender, body), ct);
        if (!result.Succeeded)
        {
            message.MarkAttemptFailed(result.FailureReason ?? ProviderError, now, Backoff, options.Value.MaxAttempts, result.IsPermanent);
            LogFailed(logger, message.Id, message.EventCode);
            return;
        }

        if (payingBrand is { } paying)
        {
            db.CreditTransactions.Add(CreditTransaction.Consume(ids.New(), paying, credits, balance, message.Id, now));
            var after = balance - credits;
            run.Balances[paying] = after;
            if (balance >= PlatformDefaults.LowCreditThreshold && after < PlatformDefaults.LowCreditThreshold)
            {
                run.CrossedLow.Add(paying);
            }
        }

        message.MarkSent(body, result.ProviderMessageId, credits);
    }

    private async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct)
    {
        try
        {
            return await sms.SendAsync(message, ct);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Sağlayıcıya ulaşılamadı (ağ, zaman aşımı): geçici hata, geri çekilmeyle yeniden denenir.
            LogProviderException(logger, exception.GetType().Name);
            return new SmsSendResult(Succeeded: false, ProviderMessageId: null, FailureReason: ProviderError);
        }
    }

    private Duration Backoff(int failedAttempts) =>
        RetryBackoff.Delay(
            failedAttempts,
            Duration.FromSeconds(options.Value.RetryBaseDelaySeconds),
            Duration.FromSeconds(options.Value.RetryMaxDelaySeconds));

    [LoggerMessage(Level = LogLevel.Warning, Message = "Notification {NotificationId} ({EventCode}) failed to send")]
    private static partial void LogFailed(ILogger logger, Guid notificationId, string eventCode);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Notification provider call threw {ExceptionType}")]
    private static partial void LogProviderException(ILogger logger, string exceptionType);

    /// <summary>Bir çalışmanın marka bazlı durumu: kilitlenmiş bakiyeler, atlanan sayılar, eşiği geçen markalar.</summary>
    private sealed class RunState
    {
        public Dictionary<Guid, int> Balances { get; } = [];

        public Dictionary<Guid, int> Skipped { get; } = [];

        public HashSet<Guid> CrossedLow { get; } = [];
    }
}
