namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeSaveBrandRequest.</summary>
/// <param name="BrandSlug">QR veya bağlantıdan çıkarılan bağlantı adı.</param>
/// <param name="Source">Qr veya Link.</param>
internal sealed record MeSaveBrandRequest(string? BrandSlug, string? Source);
