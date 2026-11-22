namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Platform paneli marka detayı (Tenancy alanları).</summary>
/// <param name="Brand">Marka satırı.</param>
/// <param name="Branches">Etkin şubeler (açılış sırasıyla).</param>
/// <param name="BookingUrl">Rezervasyon bağlantısı.</param>
public sealed record PlatformBrandProfile(PlatformBrandRow Brand, IReadOnlyList<BranchSummary> Branches, Uri BookingUrl);
