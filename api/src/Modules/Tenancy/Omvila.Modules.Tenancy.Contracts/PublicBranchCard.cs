namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Randevu ekranlarındaki salon ve şube bilgisi (sözleşme PublicAppointment.brand, branch, timeZone, preAppointmentNote).</summary>
/// <param name="BrandName">Salon adı.</param>
/// <param name="BrandSlug">Bağlantı adı.</param>
/// <param name="LogoUrl">Logo.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BranchName">Şube adı.</param>
/// <param name="AddressLine">Adres.</param>
/// <param name="District">İlçe.</param>
/// <param name="Phone">Şube telefonu.</param>
/// <param name="Latitude">Enlem.</param>
/// <param name="Longitude">Boylam.</param>
/// <param name="TimeZoneId">IANA saat dilimi.</param>
/// <param name="PreAppointmentNote">"Randevu öncesi not" (rezervasyon sayfası ayarı).</param>
public sealed record PublicBranchCard(
    string BrandName,
    string BrandSlug,
    string? LogoUrl,
    Guid BranchId,
    string BranchName,
    string AddressLine,
    string District,
    string? Phone,
    decimal? Latitude,
    decimal? Longitude,
    string TimeZoneId,
    string? PreAppointmentNote);
