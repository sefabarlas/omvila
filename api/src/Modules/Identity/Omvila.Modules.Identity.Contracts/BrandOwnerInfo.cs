namespace Omvila.Modules.Identity.Contracts;

/// <summary>Markanın sahibi.</summary>
/// <param name="FullName">Ad (yoksa boş).</param>
/// <param name="PhoneE164">Telefon (E.164).</param>
public sealed record BrandOwnerInfo(string FullName, string PhoneE164);
