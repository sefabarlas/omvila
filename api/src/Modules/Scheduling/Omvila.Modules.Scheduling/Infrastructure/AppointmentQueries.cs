using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// <see cref="IAppointmentQueries"/>: müşteri kartı randevuları (yeniden eskiye, opak imleç) ve yaklaşan randevu (toplu,
/// tek sorgu), bildirim bilgisi. Personel adları Tenancy'den toplu okunur.
/// </summary>
internal sealed class AppointmentQueries(SchedulingDbContext db, ITenantQueries tenants) : IAppointmentQueries
{
    public async Task<Result<CustomerAppointmentPage>> ListForCustomerAsync(
        Guid brandId, Guid brandCustomerId, Guid? onlyStaffId, int limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        (Instant Start, Guid Id)? after = null;
        if (cursor is not null)
        {
            if (TryDecode(cursor, out var decoded))
            {
                after = decoded;
            }
            else
            {
                errors["cursor"] = ["invalid_format"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var query = db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && a.BrandCustomerId == brandCustomerId && a.Status != AppointmentStatus.Draft);
        if (onlyStaffId is { } staffId)
        {
            query = query.Where(a => a.Items.Any(i => i.StaffId == staffId));
        }

        if (after is { } c)
        {
            query = query.Where(a => a.StartUtc < c.Start || (a.StartUtc == c.Start && a.Id.CompareTo(c.Id) < 0));
        }

        var rows = await query.Include(a => a.Items)
            .OrderByDescending(a => a.StartUtc).ThenByDescending(a => a.Id)
            .Take(limit + 1)
            .ToListAsync(ct);
        var page = rows.Take(limit).ToList();
        var staffIds = page.SelectMany(a => a.Items).Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var items = page.Select(a =>
        {
            var ordered = a.Items.OrderBy(i => i.SeqNo).ToList();
            return new CustomerAppointmentInfo(
                a.Id,
                a.BranchId,
                a.StartUtc,
                a.Status.ToString(),
                a.DisplayStatus.ToString(),
                [.. ordered.Select(i => i.ServiceNameSnapshot)],
                [.. ordered.Where(i => i.StaffId is not null && staff.ContainsKey(i.StaffId.Value)).Select(i => staff[i.StaffId!.Value].FullName).Distinct(StringComparer.Ordinal)],
                a.TotalAmountMinor,
                a.CurrencyCode);
        }).ToList();
        var next = rows.Count > limit ? Encode(page[^1].StartUtc, page[^1].Id) : null;
        return new CustomerAppointmentPage(items, next);
    }

    public async Task<IReadOnlyDictionary<Guid, Instant>> GetNextAppointmentsAsync(
        Guid brandId, IReadOnlyCollection<Guid> brandCustomerIds, Instant fromUtc, CancellationToken ct)
    {
        if (brandCustomerIds.Count == 0)
        {
            return new Dictionary<Guid, Instant>();
        }

        var rows = await db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && brandCustomerIds.Contains(a.BrandCustomerId) && a.StartUtc >= fromUtc
                && (a.Status == AppointmentStatus.PendingApproval || a.Status == AppointmentStatus.Confirmed))
            .GroupBy(a => a.BrandCustomerId)
            .Select(g => new { BrandCustomerId = g.Key, Next = g.Min(a => a.StartUtc) })
            .ToListAsync(ct);
        return rows.ToDictionary(r => r.BrandCustomerId, r => r.Next);
    }

    public async Task<IReadOnlyDictionary<Guid, Instant>> GetStartTimesAsync(Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct)
    {
        if (appointmentIds.Count == 0)
        {
            return new Dictionary<Guid, Instant>();
        }

        return await db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && appointmentIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.StartUtc, ct);
    }

    public async Task<AppointmentNotificationInfo?> GetNotificationInfoAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        var appointment = await db.Appointments.AsNoTracking().Include(a => a.Items)
            .SingleOrDefaultAsync(a => a.BrandId == brandId && a.Id == appointmentId, ct);
        if (appointment is null)
        {
            return null;
        }

        var ordered = appointment.Items.OrderBy(i => i.SeqNo).ToList();
        var staffIds = ordered.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var code = await db.AppointmentAccessCodes.AsNoTracking()
            .Where(c => c.AppointmentId == appointmentId && c.Purpose == AccessCodePurpose.Manage && c.RevokedUtc == null)
            .OrderByDescending(c => c.CreatedUtc)
            .Select(c => c.Code)
            .FirstOrDefaultAsync(ct);
        return new AppointmentNotificationInfo(
            appointment.Id,
            appointment.BranchId,
            appointment.BrandCustomerId,
            appointment.Status.ToString(),
            appointment.StartUtc,
            [.. ordered.Select(i => i.ServiceNameSnapshot)],
            [.. staffIds.Where(staff.ContainsKey).Select(id => staff[id].DisplayName ?? staff[id].FullName).Distinct(StringComparer.Ordinal)],
            code,
            AwaitedDeposit(appointment));
    }

    public async Task<IReadOnlyDictionary<Guid, AppointmentPaymentContext>> GetPaymentContextsAsync(
        Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct)
    {
        if (appointmentIds.Count == 0)
        {
            return new Dictionary<Guid, AppointmentPaymentContext>();
        }

        var appointments = await db.Appointments.AsNoTracking().Include(a => a.Items)
            .Where(a => a.BrandId == brandId && appointmentIds.Contains(a.Id))
            .ToListAsync(ct);
        var items = appointments.ToDictionary(
            a => a.Id, a => a.Items.Where(i => i.Status != AppointmentItemStatus.Cancelled).OrderBy(i => i.SeqNo).ToList());
        var firstStaff = items.Values.Select(i => i.Select(x => x.StaffId).FirstOrDefault(s => s is not null))
            .Where(s => s is not null).Select(s => s!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, firstStaff, ct);
        return appointments.ToDictionary(a => a.Id, a =>
        {
            var ordered = items[a.Id];
            var staffName = ordered.Select(i => i.StaffId).FirstOrDefault(s => s is not null) is { } first && staff.TryGetValue(first, out var card)
                ? card.FullName
                : null;
            return new AppointmentPaymentContext(a.Id, a.StartUtc, [.. ordered.Select(i => i.ServiceNameSnapshot)], staffName);
        });
    }

    public async Task<IReadOnlyDictionary<Guid, AppointmentItemContext>> GetItemContextsAsync(
        Guid brandId, IReadOnlyCollection<Guid> appointmentItemIds, CancellationToken ct)
    {
        if (appointmentItemIds.Count == 0)
        {
            return new Dictionary<Guid, AppointmentItemContext>();
        }

        var appointments = await db.Appointments.AsNoTracking().Include(a => a.Items)
            .Where(a => a.BrandId == brandId && a.Items.Any(i => appointmentItemIds.Contains(i.Id)))
            .ToListAsync(ct);
        return appointments
            .SelectMany(a => a.Items.Where(i => appointmentItemIds.Contains(i.Id))
                .Select(i => new AppointmentItemContext(i.Id, a.Id, a.StartUtc, a.BrandCustomerId, i.ServiceNameSnapshot)))
            .ToDictionary(c => c.AppointmentItemId);
    }

    public async Task<IReadOnlyList<StaffAppointmentOutcome>> ListStaffOutcomesAsync(
        Guid brandId, Guid staffId, Instant fromUtc, Instant toUtc, CancellationToken ct)
    {
        AppointmentStatus[] outcomes = [AppointmentStatus.Completed, AppointmentStatus.NoShow, AppointmentStatus.CheckedIn];
        var rows = await db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && a.StartUtc >= fromUtc && a.StartUtc < toUtc && outcomes.Contains(a.Status)
                && a.Items.Any(i => i.StaffId == staffId && i.Status != AppointmentItemStatus.Cancelled))
            .OrderBy(a => a.StartUtc).ThenBy(a => a.Id)
            .Select(a => new { a.Id, a.BrandCustomerId, a.StartUtc, a.Status })
            .ToListAsync(ct);
        return [.. rows.Select(r => new StaffAppointmentOutcome(r.Id, r.BrandCustomerId, r.StartUtc, r.Status.ToString()))];
    }

    private static AwaitedDepositInfo? AwaitedDeposit(Appointment a) =>
        a is { DisplayStatus: DisplayStatus.DepositPending, DepositRequiredMinor: { } amount, DepositTransferCode: { } code, HoldExpiresUtc: { } due }
            ? new AwaitedDepositInfo(
                new Money(amount, Enum.TryParse<Currency>(a.CurrencyCode, out var currency) ? currency : PlatformDefaults.DefaultCurrency), code, due)
            : null;

    private static string Encode(Instant start, Guid id) =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes(
            $"{start.ToUnixTimeTicks().ToString(CultureInfo.InvariantCulture)}|{id:D}"));

    private static bool TryDecode(string cursor, out (Instant Start, Guid Id) value)
    {
        value = default;
        try
        {
            var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor)).Split('|');
            if (parts.Length == 2
                && long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks)
                && Guid.TryParse(parts[1], out var id))
            {
                value = (Instant.FromUnixTimeTicks(ticks), id);
                return true;
            }
        }
        catch (FormatException)
        {
        }

        return false;
    }
}
