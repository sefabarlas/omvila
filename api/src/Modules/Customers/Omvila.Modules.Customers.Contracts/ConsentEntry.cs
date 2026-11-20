using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Eklenecek rıza satırı. Kayıtlar güncellenmez; her rıza veya geri çekme yeni satırdır.</summary>
/// <param name="CustomerId">Global kişi.</param>
/// <param name="BrandId">Markaya özel rıza; null = platform geneli (yalnız platform bağlamı okur).</param>
/// <param name="Type">Rıza türü.</param>
/// <param name="IsGranted">Verildi mi, geri mi çekildi.</param>
/// <param name="DocumentVersion">Aydınlatma metni sürümü (en çok 20 karakter).</param>
/// <param name="Channel">Kanal.</param>
/// <param name="IpAddress">İstemci IP'si (varsa).</param>
/// <param name="OccurredUtc">Zaman.</param>
public sealed record ConsentEntry(
    Guid CustomerId, Guid? BrandId, ConsentType Type, bool IsGranted, string DocumentVersion, ConsentChannel Channel, string? IpAddress, Instant OccurredUtc);
