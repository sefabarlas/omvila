namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Rezervasyon sayfası seçenekleri (sureler-ve-ayarlar.md AllowStaffSelection, AllowAnyStaffOption; BookingPageSettings.ShowPrices).</summary>
/// <param name="AllowStaffSelection">Müşteri personel seçebilir.</param>
/// <param name="AllowAnyStaffOption">"Fark etmez" seçeneği gösterilir.</param>
/// <param name="ShowPrices">Fiyatlar gösterilir.</param>
public sealed record PublicBookingOptions(bool AllowStaffSelection, bool AllowAnyStaffOption, bool ShowPrices);
