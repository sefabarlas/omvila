namespace Omvila.Modules.Sales.Contracts;

/// <summary>Paket rozeti.</summary>
/// <param name="CustomerPackageId">Müşteri paketi.</param>
/// <param name="PackageName">Paket adı (satıştaki kopya).</param>
/// <param name="RemainingSessions">Kalan seans (defterden; en az 1).</param>
public sealed record PackageBadge(Guid CustomerPackageId, string PackageName, int RemainingSessions);
