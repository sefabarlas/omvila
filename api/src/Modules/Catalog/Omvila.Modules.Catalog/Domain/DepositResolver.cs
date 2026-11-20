using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Bir kalemin kaporası (sureler-ve-ayarlar.md § Kapora, "Bir kalem için kapora çözümü" birebir).
/// Yüzde kapora liraya yuvarlanır (en yakın tam lira, AwayFromZero): havale tutarı kuruşlu olmasın.
/// StartingFrom fiyatlı hizmette yüzde en düşük (gösterilen) fiyattan hesaplanır.
/// </summary>
internal static class DepositResolver
{
    private const long MinorPerLira = 100;

    public static Money? Resolve(DepositRule rule, Money itemPrice, DepositContext context)
    {
        ArgumentNullException.ThrowIfNull(rule);
        ArgumentNullException.ThrowIfNull(context);
        if (!context.HasActiveBankAccount || !context.IsOnlineChannel || context.IsPackageRedemption)
        {
            return null;
        }

        var applies = rule.Mode switch
        {
            DepositMode.Never => false,
            DepositMode.Always => true,
            _ => context.BrandPolicy switch
            {
                BrandDepositPolicy.Always => true,
                BrandDepositPolicy.RiskyCustomersOnly => context.CustomerNoShowCount >= context.RiskyCustomerNoShowThreshold,
                _ => false,
            },
        };

        return applies ? Amount(rule, itemPrice) : null;
    }

    private static Money? Amount(DepositRule rule, Money itemPrice)
    {
        if (rule.Amount is { } amount)
        {
            return amount;
        }

        if (rule.Percent is not { } percent)
        {
            return null;
        }

        var liras = decimal.Round(itemPrice.AmountMinor * percent / 100m / MinorPerLira, 0, MidpointRounding.AwayFromZero);
        return new Money((long)liras * MinorPerLira, itemPrice.Currency);
    }
}
