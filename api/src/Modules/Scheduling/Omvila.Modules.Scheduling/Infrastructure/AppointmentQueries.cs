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
/// tek sorgu). Personel adları Tenancy'den toplu okunur.
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
