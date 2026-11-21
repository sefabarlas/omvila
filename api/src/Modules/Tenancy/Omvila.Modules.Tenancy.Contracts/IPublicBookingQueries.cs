namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Rezervasyon sayfası (public yüzey) için Tenancy okumaları. Public isteklerde jeton yoktur; kiracı bağlamı yol
/// parametresindeki şubeden kurulur (ADR-0006: "slug'dan veya erişim kodundan"; şube kimliği slug gibi herkese açık
/// bağlantı parçasıdır). İstekten (gövde, başlık) marka kimliği okunmaz.
/// </summary>
public interface IPublicBookingQueries
{
    /// <summary>
    /// Şubeyi platform bağlamında bulur ve oturumun kiracı bağlamını şubenin markasına kurar. Şube yoksa, pasifse veya
    /// marka arşivlendiyse null döner ve bağlam değişmez.
    /// </summary>
    /// <param name="branchId">Yol parametresindeki şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PublicBranchScope?> EnterBranchAsync(Guid branchId, CancellationToken ct);

    /// <summary>Rezervasyon sayfasının marka seçenekleri ("Personel seçimi", "Fark etmez", fiyat gösterimi). Marka bağlamında.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PublicBookingOptions> GetBookingOptionsAsync(Guid brandId, CancellationToken ct);

    /// <summary>Şubede etkin çalışan, etkin ve online randevuya açık personel (ada göre sıralı). Marka bağlamında.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<PublicStaffCard>> GetBookableStaffAsync(Guid brandId, Guid branchId, CancellationToken ct);

    /// <summary>Randevu ekranlarındaki salon ve şube kartı (sözleşme PublicAppointment.brand/branch). Marka bağlamında; yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="branchId">Şube.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PublicBranchCard?> GetBranchCardAsync(Guid brandId, Guid branchId, CancellationToken ct);

    /// <summary>SMS bağlantısı: <c>{Tenancy:BookingPageBaseUrl}r/{accessCode}</c> (alan adı yapılandırmadan, S-14).</summary>
    /// <param name="accessCode">Erişim kodu.</param>
    Uri AppointmentPageUrl(string accessCode);

    /// <summary>Rezervasyon sayfası bağlantısı: <c>{Tenancy:BookingPageBaseUrl}{slug}</c> (SMS'teki "rezervasyon bağlantısı").</summary>
    /// <param name="brandSlug">Markanın güncel bağlantı adı.</param>
    Uri BookingPageUrl(string brandSlug);

    /// <summary>Bekleme teklifi bağlantısı (<c>{BookingPageBaseUrl}/b/{code}</c>; bildirimler.md § Bağlantılar).</summary>
    /// <param name="accessCode">Teklifin erişim kodu.</param>
    Uri WaitlistOfferPageUrl(string accessCode);
}
