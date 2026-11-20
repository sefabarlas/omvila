using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Telefonla müşteri bul veya oluştur (online randevu, panelden randevu).</summary>
/// <param name="BrandId">Marka (kiracı bağlamıyla aynı olmalı).</param>
/// <param name="Phone">Normalleştirilmiş telefon.</param>
/// <param name="FullName">Girilen ad. Global kişi yeni oluşuyorsa onun adı olur; markada yeni kart açılıyorsa kartın adı olur.</param>
/// <param name="Source">Markada yeni kart açılırsa ilk kanal.</param>
/// <param name="PhoneVerified">Telefon OTP ile doğrulandı (online randevu). Global kişi doğrulanmış işaretlenir.</param>
public sealed record EnsureBrandCustomerRequest(Guid BrandId, PhoneNumber Phone, string FullName, CustomerSource Source, bool PhoneVerified);
