namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Rezervasyon sayfası seçenekleri (sureler-ve-ayarlar.md AllowStaffSelection, AllowAnyStaffOption; BookingPageSettings.ShowPrices).</summary>
/// <param name="AllowStaffSelection">Müşteri personel seçebilir.</param>
/// <param name="AllowAnyStaffOption">"Fark etmez" seçeneği gösterilir.</param>
/// <param name="ShowPrices">Fiyatlar gösterilir.</param>
/// <param name="DepositPolicy">Marka kapora politikası (None, RiskyCustomersOnly, Always; sureler-ve-ayarlar.md § Kapora).</param>
/// <param name="DepositAvailable">Aktif kapora hesabı (IBAN) var mı; yoksa kapora istenmez.</param>
public sealed record PublicBookingOptions(
    bool AllowStaffSelection, bool AllowAnyStaffOption, bool ShowPrices, string DepositPolicy, bool DepositAvailable);
