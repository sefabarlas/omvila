using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// ExtendRecurringSeries (durum-makinesi.md § Sistem işleri: "her gün 03:00 yerel, seriyi 3 ay ileriye üretir"; alan-modeli.md
/// § AppointmentSeries). Saatlik çalışır; şubenin yerel saati 03:00'ü geçtiyse o günün ufku (bugün + 3 ay, hariç) esas
/// alınır, 03:00'ten önce dünün ufku: böylece her seri günde bir kez, yerel 03:00'ten sonraki ilk çalışmada ilerler ve iş
/// tekrar çalışsa da aynı tarihi iki kez üretmez (<see cref="AppointmentSeries.GeneratedUntil"/>).
/// <para>
/// Her yeni tekrar <see cref="AppointmentWriter"/> ile panel kipinde yazılır (kilit, çakışma, saat dışı ve izin denetimi);
/// yazılamayan tarih atlanır (oluşturmadaki gibi). Kilit zaman aşımında seri o tarihte durur, ufku yalnız işlenen tarihe
/// kadar ilerler ve sonraki çalışmada kaldığı yerden devam eder. Yazılan her tekrar için <see cref="AppointmentConfirmed"/>
/// yayımlanır (onay SMS'i yok, hatırlatmalar planlanır). Atlanan tarihler için salona bildirim kanalı (panel bildirimi) D3'te
/// (T-53); şimdilik yalnız sayısı günlüğe yazılır.
/// </para>
/// </summary>
internal sealed partial class ExtendRecurringSeries(
    SchedulingDbContext db,
    IDbSession session,
    AppointmentWriter writer,
    ITenantQueries tenants,
    IOutbox outbox,
    IClock clock,
    ILogger<ExtendRecurringSeries> logger) : IPeriodicJob
{
    private static readonly LocalTime DailyRunLocalTime = new(3, 0);

    public string Key => "ExtendRecurringSeries";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var candidates = await db.AppointmentSeries.AsNoTracking()
            .Where(s => s.Status == SeriesStatus.Active && (s.EndsOn == null || s.GeneratedUntil < s.EndsOn))
            .Select(s => new { s.Id, s.BrandId })
            .ToListAsync(ct);

        foreach (var brand in candidates.GroupBy(c => c.BrandId))
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brand.Key), ct);
            foreach (var candidate in brand)
            {
                await ExtendAsync(candidate.Id, now, ct);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    private async Task ExtendAsync(Guid seriesId, Instant now, CancellationToken ct)
    {
        var series = await db.AppointmentSeries.SingleAsync(s => s.Id == seriesId, ct);
        var zoneId = await tenants.GetBranchTimeZoneAsync(series.BrandId, series.BranchId, ct);
        if (zoneId is null || DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) is not { } zone)
        {
            return;
        }

        var local = now.InZone(zone);
        var today = local.TimeOfDay >= DailyRunLocalTime ? local.Date : local.Date.PlusDays(-1);
        var untilExclusive = RecurrenceGenerator.HorizonEnd(today);
        var plan = RecurrenceGenerator.Extend(
            series.StartsOn, series.LocalStartTime, zone, series.Rule, series.EndsOn, series.GeneratedUntil, untilExclusive, _ => true);
        if (plan.GeneratedUntil <= series.GeneratedUntil)
        {
            return;
        }

        var template = await TemplateAppointmentAsync(series.Id, ct);
        var items = series.TemplateItems.Select(t => new RequestedItem(t.ServiceId, t.StaffId)).ToList();
        var generatedUntil = plan.GeneratedUntil;
        var skipped = 0;
        foreach (var occurrence in plan.Planned)
        {
            var slot = new SlotRequest(series.BranchId, occurrence.StartUtc, items, SameStaffForAll: false, AvailabilityMode.Panel, AllowOutsideWorkingHours: false);
            var written = await writer.CreateAsync(
                new CreateAppointmentCommand(
                    series.BrandId,
                    slot,
                    series.CustomerId,
                    series.BrandCustomerId,
                    template?.Channel ?? AppointmentChannel.Panel,
                    template?.ClientApp ?? ClientApp.Panel,
                    CustomerNote: null,
                    StaffNote: null,
                    series.Id,
                    CreatedByUserId: null),
                ct);
            if (written.IsSuccess)
            {
                await PublishAsync(written.Value.Appointment, ct);
            }
            else if (written.Error.Kind == ErrorKind.Unavailable)
            {
                // Kilit zaman aşımı: bu tarih ve sonrası bir sonraki çalışmaya kalır.
                generatedUntil = occurrence.Date.PlusDays(-1);
                break;
            }
            else
            {
                skipped++;
            }
        }

        series.MarkGeneratedUntil(generatedUntil, now);
        await db.SaveChangesAsync(ct);
        if (skipped > 0)
        {
            LogSkipped(logger, series.Id, skipped);
        }
    }

    /// <summary>Kanal ve istemci serinin en son randevusundan (panel/telefon); seri satırı bunları taşımaz.</summary>
    private Task<TemplateRow?> TemplateAppointmentAsync(Guid seriesId, CancellationToken ct) =>
        db.Appointments.AsNoTracking()
            .Where(a => a.SeriesId == seriesId)
            .OrderByDescending(a => a.StartUtc)
            .Select(a => new TemplateRow(a.Channel, a.ClientApp))
            .FirstOrDefaultAsync(ct);

    private async Task PublishAsync(Appointment appointment, CancellationToken ct)
    {
        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return;
        }

        await outbox.EnqueueAsync(
            new AppointmentConfirmed(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc,
                appointment.Channel.ToString(), NotifyCustomer: true, appointment.SeriesId, IsSeriesStart: false),
            ct);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Recurring series {SeriesId}: {Count} occurrences skipped while extending")]
    private static partial void LogSkipped(ILogger logger, Guid seriesId, int count);

    private sealed record TemplateRow(AppointmentChannel Channel, ClientApp? ClientApp);
}
