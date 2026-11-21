using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// MarkNoShows (durum-makinesi.md § Sistem işleri, A8; dilim-02 kabul kriteri 14): onaylı, Geldi denmemiş ve başlangıcı
/// + markanın "'Gelmedi' sayılma süresi" (NoShowAfterMinutes) geçmiş randevular Gelmedi olur, müşterinin gelmeme sayacı
/// 1 artar. Adaylar platform kapsamında bulunur; her marka kendi kiracı bağlamında işlenir (ayar okuma, durum geçişi ve
/// sayaç RLS altında). Hepsi işin tek transaction'ındadır: biri düşerse (ör. aynı anda Geldi denip sürüm uymazsa) iş geri
/// alınır ve bir sonraki dakikada yeniden dener.
/// </summary>
internal sealed partial class MarkNoShows(
    SchedulingDbContext db,
    IDbSession session,
    ITenantQueries tenants,
    ICustomerDirectory customers,
    IClock clock,
    ILogger<MarkNoShows> logger) : IPeriodicJob
{
    public string Key => "MarkNoShows";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();

        // Marka ayarı en az 5 dk olsa da süre markaya göre değiştiğinden aday küme başlangıcı geçmiş olanlardır.
        var brandIds = await db.Appointments.AsNoTracking()
            .Where(a => a.Status == AppointmentStatus.Confirmed && a.CheckedInUtc == null && a.StartUtc <= now)
            .Select(a => a.BrandId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var brandId in brandIds)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            var marked = await MarkBrandAsync(brandId, now, ct);
            if (marked > 0)
            {
                LogMarked(logger, brandId, marked);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    // Kural: durum-makinesi.md A8, alan-modeli.md § BrandCustomer (NoShowCount olayla güncellenir)
    private async Task<int> MarkBrandAsync(Guid brandId, Instant now, CancellationToken ct)
    {
        if (await tenants.GetAppointmentSettingsAsync(brandId, ct) is not { } settings)
        {
            return 0;
        }

        var noShowAfter = Duration.FromMinutes(settings.NoShowAfterMinutes);
        var cutoff = now - noShowAfter;
        var due = await db.Appointments
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => a.BrandId == brandId && a.Status == AppointmentStatus.Confirmed && a.CheckedInUtc == null && a.StartUtc <= cutoff)
            .ToListAsync(ct);
        var marked = due.Where(a => a.MarkNoShow(now, noShowAfter).IsSuccess).ToList();
        if (marked.Count == 0)
        {
            return 0;
        }

        await db.SaveChangesAsync(ct);
        foreach (var appointment in marked)
        {
            var counted = await customers.ApplyCountersAsync(brandId, appointment.BrandCustomerId, new CustomerCounterChange(NoShows: 1), ct);
            if (!counted.IsSuccess)
            {
                throw new DomainException($"Gelmeme sayacı güncellenemedi: {counted.Error.Code}");
            }
        }

        return marked.Count;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Marked {Count} appointments as no-show for brand {BrandId}")]
    private static partial void LogMarked(ILogger logger, Guid brandId, int count);
}
