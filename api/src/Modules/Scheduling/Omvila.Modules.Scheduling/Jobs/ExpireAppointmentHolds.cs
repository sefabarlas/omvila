using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// ExpireAppointmentHolds (durum-makinesi.md § Sistem işleri, A5; dilim-03 kabul kriteri 4): tutma süresi dolan
/// (<c>HoldExpiresUtc &lt;= şimdi</c>) onay bekleyen talepler sistemce iptal olur (ApprovalExpired; kaporada
/// DepositNotReceived), dilimler pasifleşir, saat boşalır ve <c>AppointmentApprovalExpired</c> aynı transaction'da outbox'a
/// yazılır. Adaylar platform kapsamında kısmi indeksle (<c>ix_appointments_hold_candidates</c>) bulunur; her marka kendi
/// kiracı bağlamında işlenir. Satırlar <c>FOR UPDATE SKIP LOCKED</c> ile kilitlenir: o an onaylanmakta olan talep atlanır
/// (onay kazanır), kilitlenen talebe eşzamanlı onay 412 alır (T-63). Sayaç değişmez.
/// </summary>
internal sealed partial class ExpireAppointmentHolds(
    SchedulingDbContext db,
    IDbSession session,
    IOutbox outbox,
    IClock clock,
    ILogger<ExpireAppointmentHolds> logger) : IPeriodicJob
{
    public string Key => "ExpireAppointmentHolds";

    public Duration Interval => Duration.FromMinutes(1);

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var brandIds = await db.Appointments.AsNoTracking()
            .Where(a => a.Status == AppointmentStatus.PendingApproval && a.HoldExpiresUtc != null && a.HoldExpiresUtc <= now)
            .Select(a => a.BrandId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var brandId in brandIds)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            var expired = await ExpireBrandAsync(brandId, now, ct);
            if (expired > 0)
            {
                LogExpired(logger, brandId, expired);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    // Kural: durum-makinesi.md A5
    private async Task<int> ExpireBrandAsync(Guid brandId, Instant now, CancellationToken ct)
    {
        var ids = await ExpiredHolds.LockForBrandAsync(db, brandId, now, ct);
        if (ids.Count == 0)
        {
            return 0;
        }

        var due = await db.Appointments
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => ids.Contains(a.Id))
            .ToListAsync(ct);
        var expired = due.Where(a => a.ExpireHold(now).IsSuccess).ToList();
        if (expired.Count == 0)
        {
            return 0;
        }

        await db.SaveChangesAsync(ct);
        foreach (var appointment in expired)
        {
            await outbox.EnqueueAsync(ApprovalEvents.Expired(appointment), ct);
        }

        return expired.Count;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Expired {Count} appointment holds for brand {BrandId}")]
    private static partial void LogExpired(ILogger logger, Guid brandId, int count);
}
