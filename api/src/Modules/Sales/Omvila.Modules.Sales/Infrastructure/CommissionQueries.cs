using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary><see cref="ICommissionQueries"/>: personelin ay içindeki prim kayıtları, oluştuğu anın yerel gününe göre toplanır.</summary>
internal sealed class CommissionQueries(SalesDbContext db) : ICommissionQueries
{
    public async Task<IReadOnlyList<CommissionDayTotal>> GetDailyTotalsAsync(
        Guid brandId, Guid staffId, int year, int month, string timeZoneId, CancellationToken ct)
    {
        var entries = await MonthEntries(db, brandId, staffId, year, month).ToListAsync(ct);
        return DailyTotals(entries, DateTimeZoneProviders.Tzdb[timeZoneId]);
    }

    /// <summary>Personelin ay (PeriodYearMonth) kayıtları; ters kayıtlar dahil.</summary>
    public static IQueryable<CommissionEntry> MonthEntries(SalesDbContext db, Guid brandId, Guid staffId, int year, int month)
    {
        ArgumentNullException.ThrowIfNull(db);
        var period = (year * 100) + month;
        return db.CommissionEntries.AsNoTracking().Where(e => e.BrandId == brandId && e.StaffId == staffId && e.PeriodYearMonth == period);
    }

    // Kural: sözleşme TodayCommission.daily ("kaydı olmayan gün listede yoktur"), CommissionDayAmount ("ters kayıt varsa eksi olabilir")
    public static IReadOnlyList<CommissionDayTotal> DailyTotals(IEnumerable<CommissionEntry> entries, DateTimeZone zone) =>
        [.. entries
            .GroupBy(e => e.CreatedUtc.InZone(zone).Date)
            .OrderBy(g => g.Key)
            .Select(g => new CommissionDayTotal(g.Key, g.Sum(e => e.AmountMinor), g.First().CurrencyCode))];
}
