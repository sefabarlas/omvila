namespace Omvila.Modules.Customers.Contracts;

/// <summary>"Aktif paket" uyarısının satırı.</summary>
/// <param name="CustomerPackageId">Müşteri paketi.</param>
/// <param name="PackageName">Paket adı.</param>
/// <param name="RemainingSessions">Kalan seans.</param>
public sealed record ActivePackageInfo(Guid CustomerPackageId, string PackageName, int RemainingSessions);
