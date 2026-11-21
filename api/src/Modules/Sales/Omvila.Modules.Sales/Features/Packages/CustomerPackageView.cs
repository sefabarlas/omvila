using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme CustomerPackage.</summary>
internal sealed record CustomerPackageView(
    Guid Id,
    CustomerRefView Customer,
    Guid PackageId,
    string PackageName,
    Guid BranchId,
    bool IsAvailableAllBranches,
    Money Price,
    Instant PurchasedUtc,
    LocalDate ExpiresOn,
    Guid? SoldByStaffId,
    string? SoldByStaffName,
    string Status,
    IReadOnlyList<CustomerPackageItemView> Items,
    int TotalSessions,
    int RemainingSessions,
    Money? RefundedAmount,
    Instant? RefundedUtc,
    IReadOnlyList<PackageSessionUsageView>? Usages,
    string Version);
