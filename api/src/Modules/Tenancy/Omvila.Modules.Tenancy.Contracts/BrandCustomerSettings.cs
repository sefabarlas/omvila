namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Müşteri listesinin ve kartının ihtiyaç duyduğu marka ayarları (sureler-ve-ayarlar.md; Customers modülü okur).
/// </summary>
/// <param name="MaskCustomerPhoneFromStaff">Personel müşteri telefonunu maskeli görür (roller-yetkiler.md **).</param>
/// <param name="RiskyCustomerNoShowThreshold">"Riskli müşteri tanımı: N kez gelmeyen".</param>
/// <param name="CurrencyCode">Markanın para birimi (Brand.DefaultCurrencyCode).</param>
/// <param name="TimeZoneId">
/// "Bugün"ün hesaplandığı saat dilimi: markanın ilk (en eski) etkin şubesinin saat dilimi; şube yoksa platform varsayılanı.
/// </param>
public sealed record BrandCustomerSettings(bool MaskCustomerPhoneFromStaff, int RiskyCustomerNoShowThreshold, string CurrencyCode, string TimeZoneId);
