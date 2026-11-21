using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageDefinition.</summary>
internal sealed record PackageDefinitionView(
    Guid Id,
    string Name,
    Money Price,
    IReadOnlyList<PackageDefinitionItemView> Items,
    int TotalSessions,
    int ValidityDays,
    decimal? SaleCommissionPercent,
    bool IsAvailableAllBranches,
    bool IsActive,
    int SoldCount,
    int ActiveCount,
    string Version);
