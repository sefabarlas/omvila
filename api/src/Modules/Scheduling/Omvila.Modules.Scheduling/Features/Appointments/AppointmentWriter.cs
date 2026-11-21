using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Randevu yazma yolu (musaitlik.md § Çakışma engeli: üç katman; api/AGENTS.md "kilitler (sıralı) → süresi dolmuş
/// tutmaları iptal → müsaitliği yeniden hesapla → yaz"). Uçlar (oluşturma, saat değişikliği, tekrar) bunu çağırır; kendi
/// transaction'ını açmaz, isteğin oturumunda ve markanın kiracı bağlamında çalışır, commit etmez.
/// <para>
/// İş hatası <see cref="Result"/> olarak döner ve transaction kullanılabilir kalır: kilit zaman aşımı savepoint'le geri
/// alınır, exclusion constraint ihlali (23P01) EF'in SaveChanges savepoint'iyle geri alınıp <c>slot_taken</c> olur. Böylece
/// tekrarlayan randevu bir tekrarı atlayıp (skippedOccurrences) devam edebilir, public uç <c>meta.alternatives</c>
/// hesaplayabilir. Kayıt sürümü uymazsa (xmin) <see cref="DbUpdateConcurrencyException"/> fırlar → 412.
/// </para>
/// </summary>
internal sealed class AppointmentWriter(
    SchedulingDbContext db, AvailabilitySnapshotLoader loader, AppointmentLocks locks, IOutbox outbox, IIdGenerator ids, IClock clock)
{
    private const SlotViolation Taken = SlotViolation.StaffBusy | SlotViolation.NoStaffAvailable | SlotViolation.ResourceUnavailable;
    private const SlotViolation Warnings = SlotViolation.OutsideWorkingHours | SlotViolation.StaffOnTimeOff;

    // Kural: musaitlik.md § Katman 2, durum-makinesi.md A1
    public async Task<Result<AppointmentWriteResult>> CreateAsync(CreateAppointmentCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var slot = command.Slot;
        var guarded = await GuardAsync(command.BrandId, slot, command.CustomerId, appointment: null, ct);
        if (!guarded.IsSuccess)
        {
            return guarded.Error;
        }

        var (candidate, accepted) = guarded.Value;
        var now = clock.GetCurrentInstant();
        var plan = candidate.ToPlan();
        var appointment = Appointment.Create(
            new NewAppointment(
                command.BrandId,
                slot.BranchId,
                command.CustomerId,
                command.BrandCustomerId,
                command.Channel,
                command.ClientApp,
                command.CustomerNote,
                command.StaffNote,
                command.SeriesId,
                command.CreatedByUserId,
                command.Approval?.HoldFor(plan)),
            plan,
            ids,
            now);
        var accessCode = AppointmentAccessCode.IssueForManage(ids.New(), command.BrandId, appointment.Id, appointment.EndUtc, now);
        db.Appointments.Add(appointment);
        db.AppointmentAccessCodes.Add(accessCode);

        var saved = await SaveAsync(appointment, accessCode, ct);
        return saved.IsSuccess ? new AppointmentWriteResult(appointment, accessCode, accepted) : saved.Error;
    }

    /// <summary>
    /// A14. Kilitler eski ve yeni günün ikisi için, eski ve yeni personelin hepsine alınır; randevunun kendi eski dilimleri
    /// meşguliyet sayılmaz (C5). Randevu yoksa veya kapsam dışıysa not_found.
    /// </summary>
    // Kural: musaitlik.md § Saat değişikliği, durum-makinesi.md A14
    public async Task<Result<AppointmentWriteResult>> RescheduleAsync(RescheduleAppointmentCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var appointment = await db.Appointments
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .SingleOrDefaultAsync(a => a.Id == command.AppointmentId, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        if (command.ExpectedVersion is { } expected)
        {
            db.Entry(appointment).Property(a => a.Version).OriginalValue = expected;
            if (appointment.Version != expected)
            {
                return Error.PreconditionFailed();
            }
        }

        var items = RequestedItemsFor(appointment, command.StaffChanges);
        if (!items.IsSuccess)
        {
            return items.Error;
        }

        var slot = new SlotRequest(appointment.BranchId, command.StartUtc, items.Value, SameStaffForAll: false, command.Mode, command.AllowOutsideWorkingHours);
        var guarded = await GuardAsync(appointment.BrandId, slot, appointment.CustomerId, appointment, ct);
        if (!guarded.IsSuccess)
        {
            return guarded.Error;
        }

        var (candidate, accepted) = guarded.Value;
        var now = clock.GetCurrentInstant();
        var plan = candidate.ToPlan();
        var approvalHold = command.Actor == ChangeActor.Customer ? command.Approval?.HoldFor(plan) : null;
        var rescheduled = appointment.Reschedule(plan, command.Actor, now, command.RescheduleCutoff, ids, command.UserId, approvalHold);
        if (!rescheduled.IsSuccess)
        {
            return rescheduled.Error;
        }

        var codes = await db.AppointmentAccessCodes
            .Where(c => c.AppointmentId == appointment.Id && c.Purpose == AccessCodePurpose.Manage && c.RevokedUtc == null)
            .ToListAsync(ct);
        foreach (var code in codes)
        {
            code.FollowAppointmentEnd(appointment.EndUtc);
        }

        var saved = await SaveAsync(appointment, accessCode: null, ct);
        return saved.IsSuccess ? new AppointmentWriteResult(appointment, null, accepted) : saved.Error;
    }

    /// <summary>
    /// <c>slot_taken</c> yanıtının <c>meta.alternatives</c>'i (dilim-02/03 "Saat az önce doldu"): istenen günün aynı
    /// kurallarla uygun saatlerinden istenen başlangıca en yakın <paramref name="max"/> tanesi, artan sırada. Kilitsiz
    /// okumadır (öneri); seçilen saat yine yazma yolundan geçer. Başarısız yazmadan sonra aynı transaction'da çağrılabilir.
    /// </summary>
    public async Task<IReadOnlyList<Instant>> SuggestAlternativesAsync(
        Guid brandId, SlotRequest slot, int max, Guid? customerId, Guid? ignoreAppointmentId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(slot);
        if (max <= 0)
        {
            return [];
        }

        var request = AvailabilityRequest.Create(slot.Items, slot.SameStaffForAll, slot.Mode);
        if (!request.IsSuccess)
        {
            return [];
        }

        // Ön okuma istenen yerel günü kapsar (UTC günü ± 1).
        var snapshot = await LoadAroundAsync(brandId, slot, staffIds: null, ct);
        if (snapshot is null)
        {
            return [];
        }

        var date = slot.StartUtc.InZone(snapshot.Zone).Date;
        var slots = AvailabilityEngine.GetSlots(snapshot, Ask(request.Value, slot, customerId, ignoreAppointmentId), date).Slots;
        return [.. slots
            .Select(s => s.Start)
            .Where(s => s != slot.StartUtc)
            .OrderBy(s => Math.Abs((s - slot.StartUtc).TotalTicks))
            .ThenBy(s => s)
            .Take(max)
            .Order()];
    }

    /// <summary>Sözleşmedeki <c>meta.alternatives</c> biçimi: UTC ISO 8601 metinleri.</summary>
    public static IReadOnlyDictionary<string, object?> AlternativesMeta(IReadOnlyList<Instant> alternatives) =>
        new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["alternatives"] = alternatives.Select(InstantPattern.ExtendedIso.Format).ToArray(),
        };

    /// <summary>
    /// Kural: musaitlik.md § Katman 2 adım 1–3. (1) Şubenin saat dilimi ve aday personel için kilitsiz ön okuma; (2) aday
    /// personel × yerel gün kilitleri (sıralı); (3) bu personelin süresi dolmuş tutmalarını iptal; (4) anlık görüntüyü
    /// kilitten sonra, yalnız kilitli personelle oku; (5) başlangıcı motorla değerlendir.
    /// </summary>
    private async Task<Result<(SlotCandidate Candidate, SlotViolation Accepted)>> GuardAsync(
        Guid brandId, SlotRequest slot, Guid customerId, Appointment? appointment, CancellationToken ct)
    {
        var request = AvailabilityRequest.Create(slot.Items, slot.SameStaffForAll, slot.Mode);
        if (!request.IsSuccess)
        {
            return request.Error;
        }

        var fixedStaff = FixedStaff(slot);
        var probe = await LoadAroundAsync(brandId, slot, fixedStaff, ct);
        if (probe is null)
        {
            return Error.NotFound();
        }

        var date = slot.StartUtc.InZone(probe.Zone).Date;
        var staff = new HashSet<Guid>(fixedStaff ?? CandidateStaff(probe, slot));
        var dates = new HashSet<LocalDate> { date };
        if (appointment is not null)
        {
            // Eski personelin eski günü de kilitlenir: aynı anda o saate yazmak isteyen, bu değişiklik bitene kadar bekler.
            staff.UnionWith(appointment.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value));
            dates.Add(appointment.StartUtc.InZone(probe.Zone).Date);
        }

        // Kaynak tabloları henüz yok (T-48); kaynak kilidi anahtarları envanter gelince buradan eklenir (T-49).
        var locked = await locks.AcquireAsync(AppointmentLocks.OrderedKeys(staff, [], dates), ct);
        if (!locked.IsSuccess)
        {
            return locked.Error;
        }

        await ExpireHoldsAsync(staff, dates, probe.Zone, ct);

        var snapshot = await loader.LoadAsync(brandId, slot.BranchId, date, date, ServiceIds(slot), [.. staff], ct);
        if (snapshot is null)
        {
            return Error.NotFound();
        }

        var evaluation = AvailabilityEngine.Evaluate(snapshot, Ask(request.Value, slot, customerId, appointment?.Id), slot.StartUtc);
        if (!evaluation.IsAllowed(slot.OverridesWarnings))
        {
            return MapViolations(evaluation, slot.Mode);
        }

        return (evaluation.Candidate!, evaluation.Violations & Warnings);
    }

    /// <summary>
    /// Kural: durum-makinesi.md § Sistem işleri ("çakışma kontrolünden önce, aynı personelin süresi dolmuş tutmaları satır
    /// içinde iptal edilir") ve M17. Kısıt pasif olmayan dilimi kapsadığından iptal, yeni dilimler yazılmadan önce
    /// kaydedilir. Her iptal için <c>AppointmentApprovalExpired</c> (işle aynı olay) aynı transaction'da outbox'a yazılır;
    /// istek geri alınırsa iptal ve olay birlikte geri alınır, iş sonra yeniden dener.
    /// </summary>
    private async Task ExpireHoldsAsync(HashSet<Guid> staffIds, HashSet<LocalDate> dates, DateTimeZone zone, CancellationToken ct)
    {
        if (staffIds.Count == 0)
        {
            return;
        }

        var now = clock.GetCurrentInstant();
        var staff = staffIds.ToList();
        var rangeStart = zone.AtStartOfDay(dates.Min().PlusDays(-1)).ToInstant();
        var rangeEnd = zone.AtStartOfDay(dates.Max().PlusDays(2)).ToInstant();
        var expiredIds = await (from segment in db.AppointmentItemSegments
                                join item in db.AppointmentItems on segment.AppointmentItemId equals item.Id
                                join appointment in db.Appointments on item.AppointmentId equals appointment.Id
                                where segment.IsActive && segment.StaffId != null && staff.Contains(segment.StaffId.Value)
                                      && segment.During.Start < rangeEnd && segment.During.End > rangeStart
                                      && appointment.Status == AppointmentStatus.PendingApproval
                                      && appointment.HoldExpiresUtc != null && appointment.HoldExpiresUtc <= now
                                select appointment.Id)
            .Distinct()
            .ToListAsync(ct);
        if (expiredIds.Count == 0)
        {
            return;
        }

        // Aynı anda onaylanan talep atlanır (onay kazanır; saat doluysa kısıt slot_taken der).
        expiredIds = await ExpiredHolds.LockAsync(db, expiredIds, now, ct);
        if (expiredIds.Count == 0)
        {
            return;
        }

        var expired = await db.Appointments
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => expiredIds.Contains(a.Id))
            .ToListAsync(ct);
        var cancelled = expired.Where(a => a.ExpireHold(now).IsSuccess).ToList();
        await db.SaveChangesAsync(ct);
        foreach (var appointment in cancelled)
        {
            await outbox.EnqueueAsync(ApprovalEvents.Expired(appointment), ct);
        }
    }

    /// <summary>
    /// Kayıt. 23P01 (aynı personele örtüşen aktif dilim) <c>slot_taken</c> olur: EF SaveChanges'i mevcut transaction'da bir
    /// savepoint içinde çalıştırır ve hatada ona geri döner; bu randevunun izlenen satırları bırakılır ki sonraki
    /// SaveChanges onları yeniden denemesin.
    /// </summary>
    private async Task<Result> SaveAsync(Appointment appointment, AppointmentAccessCode? accessCode, CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation })
        {
            Detach(appointment, accessCode);
            return Error.Conflict(ErrorCodes.SlotTaken);
        }
    }

    private void Detach(Appointment appointment, AppointmentAccessCode? accessCode)
    {
        var entities = new HashSet<object>(ReferenceEqualityComparer.Instance) { appointment };
        foreach (var item in appointment.Items)
        {
            entities.Add(item);
            foreach (var segment in item.Segments)
            {
                entities.Add(segment);
            }
        }

        if (accessCode is not null)
        {
            entities.Add(accessCode);
        }

        foreach (var entry in db.ChangeTracker.Entries().Where(e => entities.Contains(e.Entity)
                     || (e.Entity is AppointmentAccessCode code && code.AppointmentId == appointment.Id)).ToList())
        {
            entry.State = EntityState.Detached;
        }
    }

    /// <summary>
    /// Kural: musaitlik.md § Panelden elle ekleme ve sözleşme 409 açıklamaları. Çakışma (personel dolu, boş personel yok,
    /// kaynak dolu) her kipte <c>slot_taken</c>. Panelde çalışma saati dışı ve izin ayrı kodla uyarıdır (ikinci onayla
    /// geçilir). Rezervasyon sayfasında sunulmayan her saat "Saat az önce doldu"dur: <c>slot_taken</c> (alternatifli).
    /// Sözleşmede karşılığı olmayan istemci hataları (pencere dışı, ızgara dışı, hizmeti vermeyen personel) alan hatasıdır.
    /// </summary>
    private static Error MapViolations(SlotEvaluation evaluation, AvailabilityMode mode)
    {
        var v = evaluation.Violations;
        if ((v & SlotViolation.OffGrid) != SlotViolation.None)
        {
            return Error.Validation("startUtc", "invalid_format");
        }

        if ((v & SlotViolation.BeyondMaxAdvance) != SlotViolation.None)
        {
            return Error.Validation("startUtc", "out_of_range");
        }

        if ((v & (SlotViolation.NoEligibleStaff | SlotViolation.NotBookableOnline)) != SlotViolation.None)
        {
            return Error.Validation("items", "out_of_range");
        }

        if ((v & Taken) != SlotViolation.None || mode == AvailabilityMode.Public || evaluation.Candidate is null)
        {
            return Error.Conflict(ErrorCodes.SlotTaken);
        }

        return (v & SlotViolation.StaffOnTimeOff) != SlotViolation.None
            ? Error.Conflict(ErrorCodes.StaffOnTimeOff)
            : Error.Conflict(ErrorCodes.OutsideWorkingHours);
    }

    private static AvailabilityRequest Ask(AvailabilityRequest request, SlotRequest slot, Guid? customerId, Guid? ignoreAppointmentId) =>
        request with { CustomerId = customerId, PreferredStaffId = slot.PreferredStaffId, IgnoreAppointmentId = ignoreAppointmentId };

    /// <summary>Her kalemde personel belliyse o personeller; bir kalem fark etmez ise null.</summary>
    private static List<Guid>? FixedStaff(SlotRequest slot)
    {
        var common = slot.SameStaffForAll ? slot.Items.FirstOrDefault(i => i.StaffId is not null)?.StaffId : null;
        var staff = slot.Items.Select(i => i.StaffId ?? common).ToList();
        return staff.Exists(s => s is null) ? null : [.. staff.Select(s => s!.Value).Distinct()];
    }

    /// <summary>Fark etmez kalemde hizmeti bu şubede veren bütün personel kilitlenir (motor kilitli kümeden seçer).</summary>
    private static IEnumerable<Guid> CandidateStaff(AvailabilitySnapshot probe, SlotRequest slot) =>
        slot.Items.SelectMany(i => i.StaffId is { } staffId
            ? [staffId]
            : probe.Services.TryGetValue(i.ServiceId, out var service)
                ? service.OffersByStaff.Keys.Where(probe.Staff.ContainsKey)
                : []);

    private static List<Guid> ServiceIds(SlotRequest slot) => [.. slot.Items.Select(i => i.ServiceId).Distinct()];

    /// <summary>
    /// Kilitsiz ön okuma: yerel gün şubenin saat dilimine bağlı olduğundan, başlangıcın UTC gününün bir gün öncesi ve
    /// sonrası okunur (her saat diliminde yerel günü kapsar).
    /// </summary>
    private Task<AvailabilitySnapshot?> LoadAroundAsync(Guid brandId, SlotRequest slot, IReadOnlyCollection<Guid>? staffIds, CancellationToken ct)
    {
        var utcDate = slot.StartUtc.InUtc().Date;
        return loader.LoadAsync(brandId, slot.BranchId, utcDate.PlusDays(-1), utcDate.PlusDays(1), ServiceIds(slot), staffIds, ct);
    }

    /// <summary>Randevunun kalemleri (sıra numarasıyla) motor sorusu olarak; personel değişikliği verilmezse korunur.</summary>
    internal static Result<IReadOnlyList<RequestedItem>> RequestedItemsFor(Appointment appointment, IReadOnlyList<ItemStaffChange>? changes)
    {
        var ordered = appointment.Items.OrderBy(i => i.SeqNo).ToList();
        var staffByItem = ordered.ToDictionary(i => i.Id, i => i.StaffId);
        for (var index = 0; index < (changes?.Count ?? 0); index++)
        {
            var change = changes![index];
            if (!staffByItem.ContainsKey(change.ItemId))
            {
                return Error.Validation($"items[{index}].itemId", "out_of_range");
            }

            staffByItem[change.ItemId] = change.StaffId;
        }

        return ordered.Select(i => new RequestedItem(i.ServiceId, staffByItem[i.Id])).ToList();
    }
}
