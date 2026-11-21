using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.SmsDeliveryReports;

/// <summary>
/// Doğrulanmış teslim raporlarını uygular (sözleşme webhookSmsDeliveryReport; dilim-03 kriter 10). Satır sağlayıcı
/// kimliğiyle bulunur. Teslim edildi → Sent kalır, DeliveredUtc yazılır. Teslim edilemedi → kontör iade edilir, deneme
/// sınırdaysa Failed, değilse geri çekilmeyle Pending (yeniden gönderilir). Bilinmeyen kimlik, tekrar gelen veya artık
/// geçerli olmayan rapor sessizce yok sayılır (idempotent; sağlayıcı tekrar denemesin).
/// </summary>
internal sealed class SmsDeliveryReportHandler(
    IDbSession session, NotificationsDbContext db, IClock clock, IIdGenerator ids, IOptions<NotificationsOptions> options)
{
    public async Task ApplyAsync(IReadOnlyList<SmsDeliveryReport> reports, CancellationToken ct)
    {
        if (reports.Count == 0)
        {
            return;
        }

        // Sağlayıcı geri çağrısı kiracı taşımaz; mesaj kimliğinden bulunur. Rapor imzası doğrulandıktan sonra platform
        // bağlamı açılır ve yalnız raporun gösterdiği satırlara dokunulur (kiracı kimliği istekten alınmaz).
        await session.SetTenantAsync(TenantScope.Platform, ct);
        var now = clock.GetCurrentInstant();
        var providerIds = reports.Select(r => r.ProviderMessageId).Distinct(StringComparer.Ordinal).ToArray();

        // Satır kilidi: aynı raporun eşzamanlı iki teslimi kontörü iki kez iade etmez (ikincisi güncel durumu görür).
        var messages = await db.Outbox
            .FromSql($"SELECT * FROM notifications.notification_outbox WHERE provider_message_id = ANY({providerIds}) ORDER BY id FOR UPDATE")
            .ToListAsync(ct);
        var byProviderId = messages.ToLookup(m => m.ProviderMessageId!, StringComparer.Ordinal);

        foreach (var report in reports)
        {
            foreach (var message in byProviderId[report.ProviderMessageId])
            {
                if (report.Delivered)
                {
                    message.MarkDelivered(now);
                    continue;
                }

                var refund = message.MarkDeliveryFailed(report.FailureReason ?? "undelivered", now, Backoff, options.Value.MaxAttempts);
                if (refund is > 0 && message.BrandId is { } brandId)
                {
                    // Kural: T-64 — teslim edilemeyen SMS'in kontörü iade edilir; yeniden gönderim yeniden düşer.
                    var balance = await CreditLedger.LockBalanceAsync(db, brandId, ct);
                    db.CreditTransactions.Add(CreditTransaction.Refund(ids.New(), brandId, refund.Value, balance, message.Id, now));
                    await db.SaveChangesAsync(ct);
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private Duration Backoff(int failedAttempts) =>
        RetryBackoff.Delay(
            failedAttempts,
            Duration.FromSeconds(options.Value.RetryBaseDelaySeconds),
            Duration.FromSeconds(options.Value.RetryMaxDelaySeconds));
}
