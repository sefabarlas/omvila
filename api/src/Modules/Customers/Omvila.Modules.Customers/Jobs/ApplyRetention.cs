using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Jobs;

/// <summary>
/// ApplyRetention (guvenlik-kvkk.md § Saklama; sureler-ve-ayarlar.md TransactionRetentionYears, AuditLogRetentionYears; T-121).
/// İşlem kaydı saklama süresi dolan müşteri kartının kişiyle bağı anonimleşir (KVKK silmesiyle aynı yol: kart "Silinmiş müşteri",
/// yeni anonim kişiye bağlanır, randevu/ödeme/prim satırları anonim kartta kalır). Süre kartın son ziyaretinden (yoksa kartın
/// açılışından) sayılır; yaklaşan randevusu olan kart beklenir. Kart başına <c>AuditLog(Anonymize)</c>, işlemi yapan Sistem.
/// Denetim kayıtları silinmez: AuditLogRetentionYears'ı geçenlerin IP ve istemci bilgisi boşaltılır (<see cref="IAuditLogRetention"/>).
/// </summary>
internal sealed partial class ApplyRetention(
    CustomersDbContext db,
    IDbSession session,
    ITenantQueries tenants,
    IEnumerable<ICustomerErasureParticipant> participants,
    IAuditLog audit,
    IAuditLogRetention auditRetention,
    IIdGenerator ids,
    IClock clock,
    ILogger<ApplyRetention> logger) : IPeriodicJob
{
    public string Key => "ApplyRetention";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var today = now.InUtc().Date;
        // En kısa izinli süreden eski adaylar platform kapsamında bulunur; markanın kendi süresi kiracı bağlamında uygulanır.
        var widest = today.PlusYears(-PlatformDefaults.TransactionRetentionYearsAllowed.Min());
        var brandIds = await Candidates(widest).Select(b => b.BrandId).Distinct().ToListAsync(ct);
        foreach (var brandId in brandIds)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            var years = await tenants.GetTransactionRetentionYearsAsync(brandId, ct) ?? PlatformDefaults.TransactionRetentionYearsAllowed[0];
            var anonymized = await ApplyBrandAsync(brandId, today.PlusYears(-years), now, ct);
            if (anonymized > 0)
            {
                LogAnonymized(logger, brandId, anonymized);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
        var cleared = await auditRetention.ClearPersonalDetailsBeforeAsync(today.PlusYears(-PlatformDefaults.AuditLogRetentionYears).AtStartOfDayInZone(DateTimeZone.Utc).ToInstant(), ct);
        if (cleared > 0)
        {
            LogAuditCleared(logger, cleared);
        }
    }

    // Kural: guvenlik-kvkk.md § Anonimleştirme, § Saklama
    private async Task<int> ApplyBrandAsync(Guid brandId, LocalDate cutoff, Instant now, CancellationToken ct)
    {
        var cards = await Candidates(cutoff).Where(b => b.BrandId == brandId).ToListAsync(ct);
        var anonymized = 0;
        foreach (var card in cards)
        {
            var upcoming = 0;
            foreach (var participant in participants)
            {
                upcoming += (await participant.GetFootprintAsync(brandId, card.Id, now, ct)).UpcomingAppointments;
            }

            if (upcoming > 0)
            {
                continue;
            }

            var anonymous = Customer.CreateAnonymous(ids.New(), now);
            db.Customers.Add(anonymous);
            card.Anonymize(anonymous.Id, now);
            await db.SaveChangesAsync(ct);
            var erasure = new CustomerErasure(brandId, card.Id, anonymous.Id, UserId: null, now);
            foreach (var participant in participants)
            {
                await participant.EraseAsync(erasure, ct);
            }

            await audit.WriteAsync(
                new AuditEntry(brandId, AuditActorType.System, null, AuditAction.Anonymize, nameof(BrandCustomer), card.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["reason"] = "TransactionRetention",
                    ["cutoff"] = cutoff.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                }),
                ct);
            anonymized++;
        }

        return anonymized;
    }

    // Son ziyaret yoksa kartın açıldığı gün (UTC) sayılır.
    private IQueryable<BrandCustomer> Candidates(LocalDate cutoff)
    {
        var cutoffInstant = cutoff.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
        return db.BrandCustomers.Where(b => b.MergedIntoId == null && b.AnonymizedUtc == null
            && (b.LastVisitOn != null ? b.LastVisitOn < cutoff : b.CreatedUtc < cutoffInstant));
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Retention anonymized {Count} customer cards for brand {BrandId}")]
    private static partial void LogAnonymized(ILogger logger, Guid brandId, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retention cleared personal details of {Count} audit log entries")]
    private static partial void LogAuditCleared(ILogger logger, int count);
}
