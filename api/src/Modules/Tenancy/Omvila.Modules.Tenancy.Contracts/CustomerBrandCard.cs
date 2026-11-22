namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Müşteri uygulamasındaki marka kartı ("Salonlarım").</summary>
/// <param name="Id">Marka.</param>
/// <param name="Name">Marka adı.</param>
/// <param name="Slug">Güncel bağlantı adı.</param>
/// <param name="LogoUrl">Logo.</param>
/// <param name="CoverColor">Rezervasyon sayfası kapak rengi (petrol, mercan, kum).</param>
/// <param name="BookingEnabled">Online randevu açık mı (askıda, arşivde veya salt okunur abonelikte kapalı).</param>
/// <param name="Branches">Etkin şubeler, açılış sırasıyla.</param>
public sealed record CustomerBrandCard(
    Guid Id, string Name, string Slug, string? LogoUrl, string CoverColor, bool BookingEnabled, IReadOnlyList<CustomerBranchCard> Branches);
