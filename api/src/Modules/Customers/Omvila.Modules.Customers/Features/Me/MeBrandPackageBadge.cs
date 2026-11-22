namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeBrandPackageBadge.</summary>
/// <param name="CustomerPackageId">Müşteri paketi.</param>
/// <param name="PackageName">Paket adı.</param>
/// <param name="RemainingSessions">Kalan seans.</param>
internal sealed record MeBrandPackageBadge(Guid CustomerPackageId, string PackageName, int RemainingSessions);
