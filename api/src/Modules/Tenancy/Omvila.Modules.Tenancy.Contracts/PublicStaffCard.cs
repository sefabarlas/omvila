namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Rezervasyon sayfasında personel (sözleşme PublicCatalog.staff).</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="DisplayName">Görünen ad (yoksa ad soyad).</param>
/// <param name="Title">Unvan.</param>
/// <param name="PhotoUrl">Fotoğraf (modelde henüz yok, null).</param>
public sealed record PublicStaffCard(Guid StaffId, string DisplayName, string? Title, string? PhotoUrl);
