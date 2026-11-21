using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>
/// Takvim okuma (sözleşme manageGetCalendar; Main, HaftaTakvim, TakvimBos, TakvimKapali). Saatler, özel günler ve personel
/// müsaitlik yükleyicisinden (motorla aynı kurallar: "Yalnızca bu gün için aç" dahil), randevular tek sorguyla, müşteri
/// adları ve personel kartları toplu okunur; döngüde sorgu yoktur. İptal edilen randevu takvimde görünmez.
/// </summary>
internal sealed class CalendarReader(
    SchedulingDbContext db, AvailabilitySnapshotLoader loader, ICustomerDirectory customers, ITenantQueries tenants)
{
    /// <summary>Sözleşme: aralık en fazla 31 gün.</summary>
    public const int MaxDays = 31;

    // Kural: roller-yetkiler.md calendar.view (K: "Takvimim") ve calendar.view.all-staff; dilim-02 kabul kriterleri 3, 16
    public async Task<Result<CalendarView>> GetAsync(
        Guid brandId, Guid? branchId, string? from, string? to, string? staffIds, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        if (branchId is { } id && !access.Branches.Includes(id))
        {
            return Error.NotFound();
        }

        var query = ParseQuery(branchId, from, to, staffIds);
        if (!query.IsSuccess)
        {
            return query.Error;
        }

        var (fromDate, toDate, requested) = query.Value;
        // Diğer personeli göremeyen (personel) yalnız kendi kolonunu görür.
        IReadOnlyCollection<Guid>? visible = requested;
        if (!access.SeesAllStaff)
        {
            visible = access.OwnStaffId is { } own && (requested is null || requested.Contains(own)) ? [own] : [];
        }

        var snapshot = await loader.LoadAsync(brandId, branchId!.Value, fromDate, toDate, [], visible, ct);
        if (snapshot is null)
        {
            return Error.NotFound();
        }

        var zone = snapshot.Zone;
        var rangeStart = zone.AtStartOfDay(fromDate).ToInstant();
        var rangeEnd = zone.AtStartOfDay(toDate.PlusDays(1)).ToInstant();
        var staffIdsInView = snapshot.Staff.Keys.ToHashSet();
        var cards = await tenants.GetStaffCardsAsync(brandId, staffIdsInView, ct);
        var appointments = await LoadAppointmentsAsync(branchId.Value, rangeStart, rangeEnd, ct);
        var blocks = appointments
            .SelectMany(a => a.Items.Where(i => i.StaffId is { } s ? staffIdsInView.Contains(s) : access.SeesAllStaff && requested is null)
                .Select(i => (Appointment: a, Item: i)))
            .Where(x => x.Item.StartUtc < rangeEnd && x.Item.EndUtc > rangeStart)
            .ToList();
        var shownCustomers = blocks.Select(b => b.Appointment.BrandCustomerId).Distinct().ToList();
        var names = await customers.GetDisplayInfoAsync(brandId, shownCustomers, ct);
        var timeOffs = await tenants.GetTimeOffsAsync(brandId, staffIdsInView, rangeStart, rangeEnd, ct);
        var today = snapshot.Today;

        var staff = snapshot.Staff.Values
            .Select(s => (Schedule: s, Card: cards.GetValueOrDefault(s.StaffId)))
            .Where(s => s.Card is not null)
            .OrderBy(s => s.Card!.FullName, StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true))
            .ThenBy(s => s.Schedule.StaffId)
            .ToList();
        var days = Days(snapshot, fromDate, toDate, [.. staff.Select(s => s.Schedule)]);
        return new CalendarView(
            branchId.Value,
            zone.Id,
            fromDate,
            toDate,
            [.. staff.Select(s => Column(s.Schedule.StaffId, s.Card!, blocks))],
            days,
            [.. blocks.OrderBy(b => b.Item.StartUtc).ThenBy(b => b.Item.Id).Select(b => Block(b.Appointment, b.Item, names, today))],
            [.. timeOffs.Select(o => new TimeOff(o.Id, o.StaffId, o.Type, o.StartUtc, o.EndUtc, o.IsAllDay, o.Note))]);
    }

    /// <summary>Aralığa değen, iptal edilmemiş randevular (kalemler ve dilimlerle) tek sorguda.</summary>
    private async Task<List<Appointment>> LoadAppointmentsAsync(Guid branchId, Instant rangeStart, Instant rangeEnd, CancellationToken ct) =>
        await db.Appointments.AsNoTracking()
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => a.BranchId == branchId && a.StartUtc < rangeEnd && a.EndUtc > rangeStart
                && a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Draft)
            .AsSplitQuery()
            .ToListAsync(ct);

    private static CalendarAppointment Block(Appointment a, AppointmentItem item, IReadOnlyDictionary<Guid, CustomerDisplayInfo> names, LocalDate today)
    {
        var customer = names.GetValueOrDefault(a.BrandCustomerId);
        return new CalendarAppointment(
            a.Id,
            item.Id,
            item.StaffId,
            item.StartUtc,
            item.EndUtc,
            a.Status.ToString(),
            a.DisplayStatus.ToString(),
            customer?.FullName ?? string.Empty,
            item.ServiceNameSnapshot,
            customer?.IsNewOn(today) ?? false,
            a.SeriesId is not null,
            a.HoldExpiresUtc,
            [.. item.CurrentSegments.Select(s => new SegmentView(s.During.Start, s.During.End, s.StaffBusy))]);
    }

    /// <summary>
    /// Kolon özeti: randevu sayısı ve "bekleyen var" (personelin bekleme diliminde, başka bir meşgul dilimle dolmamış boşluk).
    /// </summary>
    private static CalendarStaff Column(Guid staffId, StaffCard card, List<(Appointment Appointment, AppointmentItem Item)> blocks)
    {
        var own = blocks.Where(b => b.Item.StaffId == staffId).ToList();
        var segments = own.SelectMany(b => b.Item.CurrentSegments.Select(s => (Segment: s, b.Appointment.Status))).ToList();
        var busy = IntervalSet.From(segments.Where(s => s.Segment.StaffBusy && s.Status != AppointmentStatus.NoShow).Select(s => s.Segment.During));
        var hasGap = segments.Exists(s => !s.Segment.StaffBusy && s.Status != AppointmentStatus.NoShow
            && !busy.Contains(s.Segment.During));
        return new CalendarStaff(staffId, card.FullName, card.CalendarColor, card.PhotoUrl, own.Select(b => b.Appointment.Id).Distinct().Count(), hasGap);
    }

    private static List<CalendarDay> Days(AvailabilitySnapshot snapshot, LocalDate from, LocalDate to, List<StaffSchedule> staff)
    {
        var days = new List<CalendarDay>();
        for (var date = from; date <= to; date = date.PlusDays(1))
        {
            var branchRanges = snapshot.BranchHours.RangesOn(date);
            days.Add(new CalendarDay(
                date,
                branchRanges.Count == 0,
                ClosureNote: null,
                [.. branchRanges.Select(TimeIntervalView.From)],
                [.. staff.Select(s => new CalendarStaffHours(s.StaffId, [.. AvailabilityEngine.StaffHoursOn(snapshot, s, date).Select(TimeIntervalView.From)]))]));
        }

        return days;
    }

    private static Result<(LocalDate From, LocalDate To, List<Guid>? StaffIds)> ParseQuery(Guid? branchId, string? from, string? to, string? staffIds)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (branchId is null)
        {
            errors["branchId"] = ["required"];
        }

        var fromDate = ParseDate("from", from, errors);
        var toDate = ParseDate("to", to, errors);
        if (fromDate is { } f && toDate is { } t && (t < f || Period.Between(f, t, PeriodUnits.Days).Days >= MaxDays))
        {
            errors["to"] = ["out_of_range"];
        }

        var requested = ParseStaffIds(staffIds, errors);
        return errors.Count > 0 ? Error.Validation(errors) : (fromDate!.Value, toDate!.Value, requested);
    }

    private static LocalDate? ParseDate(string field, string? value, Dictionary<string, string[]> errors)
    {
        if (value is null)
        {
            errors[field] = ["required"];
            return null;
        }

        var parsed = LocalDatePattern.Iso.Parse(value);
        if (parsed.Success)
        {
            return parsed.Value;
        }

        errors[field] = ["invalid_format"];
        return null;
    }

    private static List<Guid>? ParseStaffIds(string? value, Dictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var ids = new List<Guid>();
        foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!Guid.TryParse(part, out var id))
            {
                errors["staffIds"] = ["invalid_format"];
                return null;
            }

            ids.Add(id);
        }

        return ids;
    }
}
