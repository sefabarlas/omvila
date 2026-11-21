namespace Omvila.Modules.Customers.Contracts;

/// <summary>Birleştirme girdisi.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="SourceBrandCustomerId">"Birleşecek kayıt" (arşivlenir).</param>
/// <param name="TargetBrandCustomerId">"Kalacak kayıt".</param>
/// <param name="TargetCustomerId">Kalacak kaydın global kişisi (randevunun <c>CustomerId</c>'si de buna döner).</param>
public sealed record CustomerMerge(Guid BrandId, Guid SourceBrandCustomerId, Guid TargetBrandCustomerId, Guid TargetCustomerId);
