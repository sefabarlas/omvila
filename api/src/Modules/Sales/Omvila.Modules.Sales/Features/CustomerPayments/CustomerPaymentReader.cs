using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Features.Cash;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.CustomerPayments;

/// <summary>
/// Müşteri kartı Ödemeler sekmesi (sözleşme manageListCustomerPayments): kartın bütün tahsilatları (kapora, tamamla, paket satışı),
/// yeniden eskiye, opak imleçle. Şube kapsamı dışındaki tahsilat görünmez (BranchManager yalnız kendi şubeleri). "Kapanıştan sonra"
/// işareti kasadakiyle aynı kuralla: tahsilatın şube yerel gününün kapanışı, tahsilattan önceyse.
/// </summary>
internal sealed class CustomerPaymentReader(SalesDbContext db, ITenantQueries tenants, PaymentRows rowMapper)
{
    /// <summary>Sözleşme Limit parametresi varsayılanı.</summary>
    public const int DefaultLimit = 50;

    // Kural: roller-yetkiler.md payment.collect (Ş); sözleşme manageListCustomerPayments
    public async Task<Result<CustomerPaymentPage>> ListAsync(
        Guid brandId, Guid brandCustomerId, int? limit, string? cursor, BranchScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        (Instant PaidUtc, Guid Id)? after = null;
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

        var query = db.Payments.AsNoTracking().Include(p => p.Allocations)
            .Where(p => p.BrandId == brandId && p.BrandCustomerId == brandCustomerId);
        if (!scope.All)
        {
            var branchIds = scope.BranchIds.ToList();
            query = query.Where(p => branchIds.Contains(p.BranchId));
        }

        if (after is { } c)
        {
            query = query.Where(p => p.PaidUtc < c.PaidUtc || (p.PaidUtc == c.PaidUtc && p.Id.CompareTo(c.Id) < 0));
        }

        var take = limit ?? DefaultLimit;
        var rows = await query.OrderByDescending(p => p.PaidUtc).ThenByDescending(p => p.Id).Take(take + 1).ToListAsync(ct);
        var page = rows.Take(take).ToList();
        var afterClose = await RecordedAfterCloseAsync(brandId, page, ct);
        var items = await rowMapper.MapAsync(brandId, page, p => afterClose.Contains(p.Id), ct);
        return new CustomerPaymentPage(items, rows.Count > take ? Encode(page[^1].PaidUtc, page[^1].Id) : null);
    }

    private async Task<HashSet<Guid>> RecordedAfterCloseAsync(Guid brandId, List<Payment> page, CancellationToken ct)
    {
        if (page.Count == 0)
        {
            return [];
        }

        // Bir markanın şube sayısı küçüktür; saat dilimi şube başına okunur.
        var zones = new Dictionary<Guid, DateTimeZone>();
        foreach (var branchId in page.Select(p => p.BranchId).Distinct())
        {
            var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct);
            zones[branchId] = (zoneId is null ? null : DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId))
                ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        }

        var days = page.Select(p => (p.Id, p.BranchId, p.PaidUtc, Date: p.PaidUtc.InZone(zones[p.BranchId]).Date)).ToList();
        var branchIds = days.Select(d => d.BranchId).Distinct().ToList();
        var dates = days.Select(d => d.Date).Distinct().ToList();
        var closes = await db.CashDayCloses.AsNoTracking()
            .Where(c => c.BrandId == brandId && branchIds.Contains(c.BranchId) && dates.Contains(c.BusinessDate))
            .ToListAsync(ct);
        var byDay = closes.ToDictionary(c => (c.BranchId, c.BusinessDate));
        return [.. days.Where(d => byDay.TryGetValue((d.BranchId, d.Date), out var close) && close.IsRecordedAfter(d.PaidUtc)).Select(d => d.Id)];
    }

    private static string Encode(Instant paidUtc, Guid id) =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes(string.Create(CultureInfo.InvariantCulture, $"{paidUtc.ToUnixTimeTicks()}|{id:D}")));

    private static bool TryDecode(string cursor, out (Instant PaidUtc, Guid Id) value)
    {
        value = default;
        try
        {
            var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor)).Split('|');
            if (parts.Length == 2
                && long.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
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
