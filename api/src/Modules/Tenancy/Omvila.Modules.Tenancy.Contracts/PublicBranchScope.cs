namespace Omvila.Modules.Tenancy.Contracts;

/// <summary><see cref="IPublicBookingQueries.EnterBranchAsync"/> sonucu.</summary>
/// <param name="BrandId">Şubenin markası (oturumun kiracı bağlamı bu markaya kuruldu).</param>
/// <param name="BranchId">Şube.</param>
/// <param name="TimeZoneId">Şubenin IANA saat dilimi.</param>
/// <param name="BookingEnabled">Online randevu açık mı: abonelik salt okunur veya iptal değil, marka askıda değil (durum-makinesi.md § Abonelik).</param>
public sealed record PublicBranchScope(Guid BrandId, Guid BranchId, string TimeZoneId, bool BookingEnabled);
