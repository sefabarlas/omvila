using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Panelden randevu değişiklikleri: saat/personel (A14, yalnız <see cref="AppointmentWriter"/> ile), salon iptali (A7),
/// Geldi (A10), Geldiyse geri al (A9). Kapsam dışı randevu 404; If-Match sürümü uymazsa 412. Durum geçişleri aggregate
/// metodundadır; iptal ve Geldi kilit almaz (dilim yazmaz), geri alma dilimleri yeniden etkinleştirdiği için personel/gün
/// kilidi alır.
/// </summary>
internal sealed class ManageAppointmentChanges(
    SchedulingDbContext db,
    AppointmentWriter writer,
    AppointmentLocks locks,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    IOutbox outbox,
    IPackageSessions packageSessions,
    IIdGenerator ids,
    IClock clock)
{
    private const int CancelNoteMaxLength = 300;

    /// <summary>
    /// A14 (salon; son süreye tabi değil). "Bu ve sonraki randevular" kapsamında serinin sonraki etkin tekrarları aynı yerel
    /// saate ve aynı personel değişikliğiyle taşınır, seri o tarihten bölünür; tekrarlardan biri yazılamazsa istek bütünüyle
    /// geri alınır. Bu kapsamda gün değiştirilemez (T-50).
    /// </summary>
    // Kural: durum-makinesi.md A14, musaitlik.md § Saat değişikliği, alan-modeli.md § AppointmentSeries
    public async Task<Result<Appointment>> RescheduleAsync(
        Guid brandId, Guid appointmentId, ManageRescheduleRequest body, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var changes = ValidateReschedule(body, errors);
        var scope = ParseScope(body.Scope, errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        // Personel (K) randevuyu başka personele taşıyamaz.
        if (changes.Exists(c => !access.IsOwnStaff(c.StaffId)))
        {
            return Error.Forbidden();
        }

        var previousStart = appointment.StartUtc;
        var start = body.StartUtc!.Value;
        var following = new List<Appointment>();
        DateTimeZone? zone = null;
        if (scope == SeriesScope.ThisAndFollowing && appointment.SeriesId is { } seriesId)
        {
            zone = await ZoneAsync(brandId, appointment.BranchId, ct);
            if (start.InZone(zone).Date != previousStart.InZone(zone).Date)
            {
                return Error.Validation("startUtc", "out_of_range");
            }

            following = await FollowingAsync(seriesId, previousStart, ct);
        }

        var notify = body.NotifyCustomer ?? true;
        var allowOutside = body.AllowOutsideWorkingHours ?? false;
        var moved = await writer.RescheduleAsync(
            new RescheduleAppointmentCommand(appointment.Id, start, ChangeActor.Salon, Duration.Zero, userId, changes, allowOutside, expectedVersion), ct);
        if (!moved.IsSuccess)
        {
            return moved.Error;
        }

        await PublishRescheduledAsync(appointment, previousStart, notify, ct);
        if (zone is null)
        {
            return appointment;
        }

        var newLocalTime = start.InZone(zone).TimeOfDay;
        var movedFollowing = await MoveFollowingAsync(appointment, following, zone, newLocalTime, allowOutside, userId, ct);
        return movedFollowing.IsSuccess ? await SplitSeriesAsync(appointment, following, zone, previousStart, newLocalTime, ct) : movedFollowing.Error;
    }

    /// <summary>"Bu ve sonrakiler": sonraki tekrarlar kendi günlerinde yeni yerel saate, kalem sırasına göre aynı personele.</summary>
    private async Task<Result> MoveFollowingAsync(
        Appointment selected, List<Appointment> following, DateTimeZone zone, LocalTime newLocalTime, bool allowOutside, Guid? userId, CancellationToken ct)
    {
        var staffBySeq = selected.Items.ToDictionary(i => i.SeqNo, i => i.StaffId);
        foreach (var occurrence in following)
        {
            var occurrenceStart = RecurrenceGenerator.StartOf(occurrence.StartUtc.InZone(zone).Date, newLocalTime, zone);
            var occurrenceChanges = occurrence.Items
                .Where(i => staffBySeq.TryGetValue(i.SeqNo, out var staffId) && staffId is not null && staffId != i.StaffId)
                .Select(i => new ItemStaffChange(i.Id, staffBySeq[i.SeqNo]!.Value))
                .ToList();
            var occurrencePrevious = occurrence.StartUtc;
            var result = await writer.RescheduleAsync(
                new RescheduleAppointmentCommand(occurrence.Id, occurrenceStart, ChangeActor.Salon, Duration.Zero, userId, occurrenceChanges, allowOutside), ct);
            if (!result.IsSuccess)
            {
                return result.Error;
            }

            // Seri toplu değişikliğinde müşteriye tek SMS gider (seçilen randevu); sonraki tekrarların hatırlatmaları
            // yine yeni saate taşınır (T-53).
            await PublishRescheduledAsync(occurrence, occurrencePrevious, notify: false, ct);
        }

        return Result.Success();
    }

    // Kural: durum-makinesi.md A7, dilim-02 kabul kriteri 13 (TekrarIptal: yalnız bu / bu ve sonrakiler; öncekiler durur)
    public async Task<Result<Appointment>> CancelAsync(
        Guid brandId, Guid appointmentId, ManageCancelRequest body, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var reason = ValidateCancel(body, errors);
        var scope = ParseScope(body.Scope, errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var now = clock.GetCurrentInstant();
        var cancelled = appointment.CancelBySalon(reason, body.Note, userId, now);
        if (!cancelled.IsSuccess)
        {
            return cancelled.Error;
        }

        var all = new List<Appointment> { appointment };
        if (scope == SeriesScope.ThisAndFollowing && appointment.SeriesId is not null)
        {
            var following = await CancelFollowingAsync(brandId, appointment, reason, body.Note, userId, now, ct);
            if (!following.IsSuccess)
            {
                return following.Error;
            }

            all.AddRange(following.Value);
        }

        await db.SaveChangesAsync(ct);
        await packageSessions.ReleaseAsync(brandId, all, ct);
        var notify = body.NotifyCustomer ?? true;
        var offerToWaitlist = body.OfferToWaitlist ?? true;
        foreach (var a in all)
        {
            // "Bu ve sonrakiler" iptalinde müşteriye tek SMS gider (seçilen randevu); hepsinin hatırlatmaları düşer (T-53).
            var notifyThis = notify && ReferenceEquals(a, appointment);
            await outbox.EnqueueAsync(
                new AppointmentCancelledBySalon(
                    a.BrandId, a.Id, a.BrandCustomerId, a.StartUtc, reason.ToString(), notifyThis,
                    DepositReceived: a.DepositStatus == DepositStatus.Received, OfferToWaitlist: offerToWaitlist),
                ct);
        }

        return appointment;
    }

    // Kural: durum-makinesi.md A10
    public async Task<Result<Appointment>> CheckInAsync(Guid appointmentId, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var checkedIn = appointment.CheckIn(clock.GetCurrentInstant(), userId);
        if (!checkedIn.IsSuccess)
        {
            return checkedIn.Error;
        }

        await db.SaveChangesAsync(ct);
        return appointment;
    }

    /// <summary>
    /// A9: dilimler yeniden etkinleşir. Personel/gün kilidi önce alınır (yazma yolu sırası); arada saat başkasına verildiyse
    /// exclusion constraint (23P01) <c>slot_taken</c> olur. Gelmeme sayacı düşer.
    /// </summary>
    // Kural: durum-makinesi.md A9, musaitlik.md § Katman 2
    public async Task<Result<Appointment>> UndoNoShowAsync(
        Guid brandId, Guid appointmentId, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var zone = await ZoneAsync(brandId, appointment.BranchId, ct);
        var staff = appointment.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value);
        var locked = await locks.AcquireAsync(AppointmentLocks.OrderedKeys(staff, [], [appointment.StartUtc.InZone(zone).Date]), ct);
        if (!locked.IsSuccess)
        {
            return locked.Error;
        }

        var undone = appointment.UndoNoShow(clock.GetCurrentInstant(), zone, userId);
        if (!undone.IsSuccess)
        {
            return undone.Error;
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation })
        {
            return Error.Conflict(ErrorCodes.SlotTaken);
        }

        var counters = await customers.ApplyCountersAsync(brandId, appointment.BrandCustomerId, new CustomerCounterChange(NoShows: -1), ct);
        return counters.IsSuccess ? appointment : counters.Error;
    }

    /// <summary>Serinin <paramref name="after"/> sonrasındaki etkin (onay bekleyen veya onaylı) tekrarları, başlangıç sırasıyla.</summary>
    private Task<List<Appointment>> FollowingAsync(Guid seriesId, Instant after, CancellationToken ct) =>
        db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => a.SeriesId == seriesId && a.StartUtc > after
                && (a.Status == AppointmentStatus.PendingApproval || a.Status == AppointmentStatus.Confirmed))
            .OrderBy(a => a.StartUtc)
            .ToListAsync(ct);

    private async Task<Appointment?> LoadAsync(Guid appointmentId, ManageAccess access, CancellationToken ct)
    {
        var appointment = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);
        return appointment is not null && access.Covers(appointment) ? appointment : null;
    }

    private async Task<DateTimeZone> ZoneAsync(Guid brandId, Guid branchId, CancellationToken ct)
    {
        var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
    }

    /// <summary>Seri seçilen tarihten bölünür; seçilen ve sonraki tekrarlar yeni seriye (yeni yerel saat, yeni şablon) geçer.</summary>
    private async Task<Appointment> SplitSeriesAsync(
        Appointment selected, List<Appointment> following, DateTimeZone zone, Instant previousStart, LocalTime newLocalTime, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var series = await db.AppointmentSeries.SingleAsync(s => s.Id == selected.SeriesId, ct);
        var templates = selected.Items.OrderBy(i => i.SeqNo)
            .Select(i => new SeriesItemTemplate(i.ServiceId, i.StaffId, (int)(i.StartUtc - selected.StartUtc).TotalMinutes))
            .ToList();
        var next = series.SplitFrom(previousStart.InZone(zone).Date, ids.New(), newLocalTime, templates, now);
        db.AppointmentSeries.Add(next);
        foreach (var appointment in following.Prepend(selected))
        {
            appointment.AssignSeries(next.Id, now);
        }

        await db.SaveChangesAsync(ct);
        return selected;
    }

    private Task PublishRescheduledAsync(Appointment appointment, Instant previousStart, bool notify, CancellationToken ct) =>
        outbox.EnqueueAsync(
            new AppointmentRescheduled(appointment.BrandId, appointment.Id, appointment.BrandCustomerId, previousStart, appointment.StartUtc, nameof(ChangeActor.Salon), notify),
            ct);

    /// <summary>"Bu ve sonraki randevular": sonraki etkin tekrarlar iptal, seri seçilen günden önce biter; öncekiler durur.</summary>
    private async Task<Result<List<Appointment>>> CancelFollowingAsync(
        Guid brandId, Appointment selected, CancellationReason reason, string? note, Guid? userId, Instant now, CancellationToken ct)
    {
        var seriesId = selected.SeriesId!.Value;
        var start = selected.StartUtc;
        var following = await FollowingAsync(seriesId, start, ct);
        foreach (var occurrence in following)
        {
            var result = occurrence.CancelBySalon(reason, note, userId, now);
            if (!result.IsSuccess)
            {
                return result.Error;
            }
        }

        var series = await db.AppointmentSeries.SingleAsync(s => s.Id == seriesId, ct);
        var zone = await ZoneAsync(brandId, selected.BranchId, ct);
        series.EndBefore(start.InZone(zone).Date, now);
        return following;
    }

    private static List<ItemStaffChange> ValidateReschedule(ManageRescheduleRequest body, Dictionary<string, string[]> errors)
    {
        if (body.StartUtc is null)
        {
            errors["startUtc"] = ["required"];
        }

        var changes = new List<ItemStaffChange>();
        for (var i = 0; i < (body.Items?.Count ?? 0); i++)
        {
            var item = body.Items![i];
            if (item?.ItemId is { } itemId && item.StaffId is { } staffId)
            {
                changes.Add(new ItemStaffChange(itemId, staffId));
            }
            else
            {
                errors[item?.ItemId is null ? $"items[{i}].itemId" : $"items[{i}].staffId"] = ["required"];
            }
        }

        return changes;
    }

    /// <summary>Salon nedenleri (sözleşme ManageCancelRequest.reason); listede olmayan neden alanda reddedilir.</summary>
    private static CancellationReason ValidateCancel(ManageCancelRequest body, Dictionary<string, string[]> errors)
    {
        CancellationReason reason = default;
        if (body.Reason is null)
        {
            errors["reason"] = ["required"];
        }
        else if (!Enum.TryParse(body.Reason, ignoreCase: false, out reason) || !Enum.IsDefined(reason))
        {
            errors["reason"] = ["invalid_format"];
        }

        if (body.Note is { Length: > CancelNoteMaxLength })
        {
            errors["note"] = ["too_long"];
        }

        return reason;
    }

    private static SeriesScope ParseScope(string? value, Dictionary<string, string[]> errors)
    {
        if (value is null)
        {
            return SeriesScope.ThisOnly;
        }

        if (Enum.TryParse<SeriesScope>(value, ignoreCase: false, out var scope) && Enum.IsDefined(scope))
        {
            return scope;
        }

        errors["scope"] = ["invalid_format"];
        return SeriesScope.ThisOnly;
    }
}
