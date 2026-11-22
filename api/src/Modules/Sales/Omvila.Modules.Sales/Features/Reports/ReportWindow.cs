using NodaTime;
using NodaTime.Text;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>
/// Raporun aralığı ve şube kapsamı (sözleşme ReportFrom/ReportTo: şubenin yerel takvimi, iki uç dahil, en fazla 31 gün). Şube
/// verilirse o şube (kapsam dışı veya markada yoksa 404) ve onun saat dilimi; verilmezse kullanıcının erişebildiği bütün şubeler
/// (<see cref="BranchIds"/> null = marka geneli) ve markanın "bugün" saat dilimi (T-120).
/// </summary>
internal sealed record ReportWindow(Guid? BranchId, IReadOnlyCollection<Guid>? BranchIds, DateTimeZone Zone, LocalDate From, LocalDate To, Currency Currency)
{
    /// <summary>Sözleşme: from ile to arası en fazla 31 gün.</summary>
    public const int MaxDays = 31;

    public int Days => Period.DaysBetween(From, To) + 1;

    public Instant StartUtc => Zone.AtStartOfDay(From).ToInstant();

    public Instant EndUtc => Zone.AtStartOfDay(To.PlusDays(1)).ToInstant();

    /// <summary>Bir önceki eşit uzunluktaki aralığın başı (earnedRevenueChangePercent).</summary>
    public Instant PreviousStartUtc => Zone.AtStartOfDay(From.PlusDays(-Days)).ToInstant();

    public IEnumerable<LocalDate> Dates => Enumerable.Range(0, Days).Select(From.PlusDays);

    // Kural: sözleşme ReportFrom, ReportTo (400 validation_failed, errors.to = [out_of_range]); roller-yetkiler.md report.view / commission.view (Ş)
    public static async Task<Result<ReportWindow>> ResolveAsync(
        ITenantQueries tenants, Guid brandId, Guid? branchId, BranchScope scope, string? from, string? to, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(tenants);
        ArgumentNullException.ThrowIfNull(scope);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var fromDate = Parse(from, "from", errors);
        var toDate = Parse(to, "to", errors);
        if (fromDate is { } f && toDate is { } t && (t < f || Period.DaysBetween(f, t) + 1 > MaxDays))
        {
            errors["to"] = ["out_of_range"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var settings = await tenants.GetCustomerSettingsAsync(brandId, ct) ?? throw new DomainException("Marka ayarları bulunamadı.");
        var currency = Enum.TryParse<Currency>(settings.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        if (branchId is { } id)
        {
            if (!scope.Includes(id) || await tenants.GetBranchTimeZoneAsync(brandId, id, ct) is not { } zoneId)
            {
                return Error.NotFound();
            }

            return new ReportWindow(id, [id], ZoneOf(zoneId), fromDate!.Value, toDate!.Value, currency);
        }

        return new ReportWindow(null, scope.All ? null : scope.BranchIds, ZoneOf(settings.TimeZoneId), fromDate!.Value, toDate!.Value, currency);
    }

    private static LocalDate? Parse(string? value, string field, Dictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[field] = ["required"];
            return null;
        }

        var parsed = LocalDatePattern.Iso.Parse(value);
        if (!parsed.Success)
        {
            errors[field] = ["invalid_format"];
            return null;
        }

        return parsed.Value;
    }

    private static DateTimeZone ZoneOf(string zoneId) =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
}
