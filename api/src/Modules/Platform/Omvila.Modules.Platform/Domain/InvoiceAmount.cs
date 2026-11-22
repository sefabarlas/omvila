using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Bir dönemin tutarı (T-118): plan fiyatı (KDV hariç; aylık veya yıllık) + ek şube ücreti × ek şube sayısı (yıllıkta 12 ay); KDV
/// yapılandırmadaki orandan (yoksa 0). Plan fiyatı tanımlı değilse (S-14) tutar yoktur ve fatura kesilmez.
/// </summary>
internal sealed record InvoiceAmount(long NetMinor, long TaxMinor, Currency Currency)
{
    private const int MonthsPerYear = 12;

    public long TotalMinor => NetMinor + TaxMinor;

    /// <summary>Ek şube: etkin şubelerin planın içerdiği sayıyı aşan kısmı (dahil sayı boşsa sınırsız, ek yok).</summary>
    public static int ExtraBranches(int activeBranches, int? includedBranches) =>
        includedBranches is { } included ? Math.Max(0, activeBranches - included) : 0;

    public static InvoiceAmount? Calculate(Plan plan, BillingCycle cycle, int activeBranches, decimal? vatRatePercent)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var price = cycle == BillingCycle.Yearly ? plan.YearlyPriceMinor : plan.MonthlyPriceMinor;
        if (price is not { } basePrice)
        {
            return null;
        }

        var extras = ExtraBranches(activeBranches, plan.IncludedBranchCount);
        var months = cycle == BillingCycle.Yearly ? MonthsPerYear : 1;
        var extraPrice = extras == 0 ? 0 : (plan.ExtraBranchPriceMinor ?? 0) * extras * months;
        var net = basePrice + extraPrice;
        var tax = vatRatePercent is { } rate and > 0
            ? (long)Math.Round(net * rate / 100m, MidpointRounding.AwayFromZero)
            : 0;
        return new InvoiceAmount(net, tax, PlatformDefaults.DefaultCurrency);
    }
}
