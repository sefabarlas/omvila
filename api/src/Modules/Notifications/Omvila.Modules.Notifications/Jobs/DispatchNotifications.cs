using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Jobs;

/// <summary>
/// DispatchNotifications (docs/mimari/mimari.md § Arka plan işleri): zamanı gelmiş bekleyen bildirimleri
/// şablonla oluşturup gönderir. Platform kapsamlı iş transaction'ında çalışır (JobRunner).
/// Gönderim en az bir kez: commit düşerse mesaj yeniden gönderilebilir.
/// </summary>
internal sealed partial class DispatchNotifications(
    NotificationsDbContext db,
    ISmsSender sms,
    IClock clock,
    IOptions<NotificationsOptions> options,
    IOptions<SmsOptions> smsOptions,
    ILogger<DispatchNotifications> logger) : IPeriodicJob
{
    public string Key => "DispatchNotifications";

    public Duration Interval => Duration.FromSeconds(options.Value.DispatchIntervalSeconds);

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var batchSize = options.Value.BatchSize;
        var due = await db.Outbox
            .FromSql($"SELECT * FROM notifications.notification_outbox WHERE status = 'Pending' AND scheduled_utc <= {now} ORDER BY scheduled_utc LIMIT {batchSize} FOR UPDATE SKIP LOCKED")
            .ToListAsync(ct);

        foreach (var message in due)
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Payload) ?? [];
            var body = SmsTemplates.Render(message.EventCode, values);
            var result = await sms.SendAsync(new SmsMessage(message.RecipientPhone!, smsOptions.Value.DefaultSender, body), ct);
            if (result.Succeeded)
            {
                message.MarkSent(body, result.ProviderMessageId);
            }
            else
            {
                message.MarkAttemptFailed(result.FailureReason ?? "unknown", now + Duration.FromSeconds(options.Value.RetryDelaySeconds), options.Value.MaxAttempts);
                LogFailed(logger, message.Id, message.EventCode);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Notification {NotificationId} ({EventCode}) failed to send")]
    private static partial void LogFailed(ILogger logger, Guid notificationId, string eventCode);
}
