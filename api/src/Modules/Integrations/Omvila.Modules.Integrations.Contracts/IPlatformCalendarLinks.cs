namespace Omvila.Modules.Integrations.Contracts;

/// <summary>
/// Platform paneli Google Takvim okuma servisi (marka detayı "Google Takvim · 3 personelde bağlı"; sözleşme
/// AdminBrandDetail.googleCalendarConnectedStaffCount). Platform servisidir (ADR-0006 K4): yalnız admin yüzeyinin platform
/// bağlamında çağrılır, başka bağlamda istisna atar. Yalnız sayı döner.
/// </summary>
public interface IPlatformCalendarLinks
{
    /// <summary>Markada bağlantısı etkin (Active) personel sayısı.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> CountConnectedStaffAsync(Guid brandId, CancellationToken ct);
}
