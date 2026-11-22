using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Sales.Features.Packages;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Sözleşme MePackage (Paketlerim kartı).</summary>
internal sealed record MePackage(
    Guid Id,
    MeBrandRef Brand,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? BranchName,
    bool IsAvailableAllBranches,
    string PackageName,
    string Status,
    IReadOnlyList<CustomerPackageItemView> Items,
    int TotalSessions,
    int RemainingSessions,
    LocalDate ExpiresOn,
    bool IsUsable,
    bool IsRunningLow,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? FinishedOn);
