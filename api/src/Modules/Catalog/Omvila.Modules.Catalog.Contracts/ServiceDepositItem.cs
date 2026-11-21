using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Kapora çözülecek kalem.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="Price">Kalem fiyatı (yüzde kapora bundan; StartingFrom hizmette en düşük fiyat).</param>
/// <param name="IsPackageRedemption">Kalem paketten düşülüyor mu (kapora istenmez).</param>
public sealed record ServiceDepositItem(Guid ServiceId, Money Price, bool IsPackageRedemption);
