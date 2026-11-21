using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageOverview.</summary>
internal sealed record PackageOverviewView(
    int ActiveCustomerPackages,
    int RemainingSessions,
    int SoldThisMonth,
    Money SoldThisMonthAmount,
    int ExpiringSoon,
    decimal? UsageRatePercent);
