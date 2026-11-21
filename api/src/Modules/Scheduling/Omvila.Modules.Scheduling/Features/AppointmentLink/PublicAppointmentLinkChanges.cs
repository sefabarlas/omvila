using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.Modules.Scheduling.Features.Slots;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.AppointmentLink;

/// <summary>
/// SMS bağlantısından müşteri değişiklikleri (dilim-02 kabul kriteri 10; ekranlar Randevum, Ertele, Iptal): iptal (A6,
/// <c>CancellationCutoffHours</c>, neden zorunlu) ve saat değişikliği (A14, <c>RescheduleCutoffHours</c>, yalnız
/// <see cref="AppointmentWriter"/> ile; aynı hizmetler ve aynı personel). Son süre yalnız onaylı randevuda uygulanır,
/// sınır tam eşitlikte serbesttir; geçmişse 409 <c>cutoff_passed</c> (arayüz "Salonu arayın" gösterse de API denetler).
/// Randevu oturumun markasında ve <see cref="AppointmentLinkResolver"/> ile yüklenmiş olmalıdır.
/// </summary>
internal sealed class PublicAppointmentLinkChanges(
    SchedulingDbContext db,
    AppointmentWriter writer,
    AvailabilitySnapshotLoader loader,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    IOutbox outbox,
    IClock clock)
{
    private const int CancelNoteMaxLength = 300;
    private const int AlternativeCount = 3;

    // Kural: durum-makinesi.md A6, dilim-02 § Alan kuralları (müşteri iptali: CancellationCutoffHours, neden zorunlu)
    public async Task<Result> CancelAsync(Appointment appointment, CustomerCancelRequest body, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        ArgumentNullException.ThrowIfNull(body);
        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
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

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var settings = await SettingsAsync(appointment.BrandId, ct);
        var cancelled = appointment.CancelByCustomer(reason, body.Note, clock.GetCurrentInstant(), Duration.FromHours(settings.CancellationCutoffHours));
        if (!cancelled.IsSuccess)
        {
            return cancelled.Error;
        }

        await db.SaveChangesAsync(ct);
        var counters = await customers.ApplyCountersAsync(appointment.BrandId, appointment.BrandCustomerId, new CustomerCounterChange(Cancels: 1), ct);
        if (!counters.IsSuccess)
        {
            return counters.Error;
        }

        await outbox.EnqueueAsync(
            new AppointmentCancelledByCustomer(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc, reason.ToString()),
            ct);
        return Result.Success();
    }

    /// <summary>
    /// A14 (müşteri): public kip (pencere, ızgara, en az ön bildirim, online personel), kalemlerin personeli korunur,
    /// randevunun kendi eski dilimleri meşguliyet sayılmaz. Sunulmayan saat <c>slot_taken</c> + <c>meta.alternatives</c>.
    /// Onay gerektiren salonda randevu yeniden Onay bekliyor olur, yeni tutma şimdi + ApprovalTimeoutMinutes (A13, S-11;
    /// olay <see cref="AppointmentRequested"/> IsReschedule). Kapora aktarımı Dilim 4.
    /// </summary>
    // Kural: durum-makinesi.md A14, musaitlik.md § Saat değişikliği, dilim-02 § Alan kuralları (RescheduleCutoffHours)
    public async Task<Result> RescheduleAsync(Appointment appointment, PublicRescheduleRequest body, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        ArgumentNullException.ThrowIfNull(body);
        if (body.StartUtc is not { } start)
        {
            return Error.Validation("startUtc", "required");
        }

        var settings = await SettingsAsync(appointment.BrandId, ct);
        var previousStart = appointment.StartUtc;
        var previousHold = appointment.HoldExpiresUtc;
        var moved = await writer.RescheduleAsync(
            new RescheduleAppointmentCommand(
                appointment.Id, start, ChangeActor.Customer, Duration.FromHours(settings.RescheduleCutoffHours), UserId: null,
                ExpectedVersion: expectedVersion, Approval: settings.Policy()),
            ct);
        if (!moved.IsSuccess)
        {
            return await WithAlternativesAsync(moved.Error, appointment, start, ct);
        }

        // Onay bekleyen randevuda Notifications "değişti" mesajı yazmaz, hatırlatmaları düşürür (yalnız onaylıda planlar).
        await outbox.EnqueueAsync(
            new AppointmentRescheduled(
                appointment.BrandId, appointment.Id, appointment.BrandCustomerId, previousStart, appointment.StartUtc, nameof(ChangeActor.Customer), NotifyCustomer: true),
            ct);
        if (appointment.Status == AppointmentStatus.PendingApproval && appointment.HoldExpiresUtc != previousHold)
        {
            // A13: yeni tutmayla yeniden onaya düştü ("Talebiniz alındı" ve staff.approval_needed).
            await outbox.EnqueueAsync(ApprovalEvents.Requested(appointment, isReschedule: true), ct);
        }

        return Result.Success();
    }

    /// <summary>
    /// Saat değişikliği için o günün boş saatleri: aynı hizmetler, aynı personel, public kip; randevunun kendi dilimleri
    /// boş sayılır. Randevu değiştirilemiyorsa (iptal, geçmiş durum) <c>invalid_state_transition</c>, erteleme son süresi
    /// geçmişse <c>cutoff_passed</c>.
    /// </summary>
    // Kural: musaitlik.md § Saat değişikliği, durum-makinesi.md A14
    public async Task<Result<SlotSearchResult>> SearchSlotsAsync(Appointment appointment, RescheduleSlotSearchRequest body, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        ArgumentNullException.ThrowIfNull(body);
        if (body.Date is not { } date)
        {
            return Error.Validation("date", "required");
        }

        if (appointment.Status is not (AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        var settings = await SettingsAsync(appointment.BrandId, ct);
        if (appointment.Status == AppointmentStatus.Confirmed
            && appointment.StartUtc - clock.GetCurrentInstant() < Duration.FromHours(settings.RescheduleCutoffHours))
        {
            return Error.Conflict(ErrorCodes.CutoffPassed);
        }

        var items = AppointmentWriter.RequestedItemsFor(appointment, changes: null).Value;
        var request = AvailabilityRequest.Create(items, sameStaffForAll: false, AvailabilityMode.Public);
        if (!request.IsSuccess)
        {
            return request.Error;
        }

        var staffIds = items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var snapshot = await loader.LoadAsync(
            appointment.BrandId, appointment.BranchId, date, date, [.. items.Select(i => i.ServiceId).Distinct()], staffIds.Count == items.Count ? staffIds : null, ct);
        if (snapshot is null)
        {
            return Error.NotFound();
        }

        var ask = request.Value with { CustomerId = appointment.CustomerId, IgnoreAppointmentId = appointment.Id };
        var day = AvailabilityEngine.GetSlots(snapshot, ask, date);
        var staff = await PublicAvailabilityEndpoints.PublicNamesAsync(
            tenants, appointment.BrandId, day.Slots.SelectMany(s => s.Items.Select(i => i.StaffId)).Distinct().ToList(), ct);
        return SlotMapping.ToResult(day, snapshot.Zone, staff);
    }

    private async Task<Error> WithAlternativesAsync(Error error, Appointment appointment, Instant start, CancellationToken ct)
    {
        if (!string.Equals(error.Code, ErrorCodes.SlotTaken, StringComparison.Ordinal))
        {
            return error;
        }

        var items = AppointmentWriter.RequestedItemsFor(appointment, changes: null).Value;
        var slot = new SlotRequest(appointment.BranchId, start, items, SameStaffForAll: false, AvailabilityMode.Public);
        var alternatives = await writer.SuggestAlternativesAsync(appointment.BrandId, slot, AlternativeCount, appointment.CustomerId, appointment.Id, ct);
        return Error.Conflict(ErrorCodes.SlotTaken, AppointmentWriter.AlternativesMeta(alternatives));
    }

    private async Task<BrandAppointmentSettings> SettingsAsync(Guid brandId, CancellationToken ct) =>
        await tenants.GetAppointmentSettingsAsync(brandId, ct) ?? throw new DomainException("Markanın randevu ayarları yok.");
}
