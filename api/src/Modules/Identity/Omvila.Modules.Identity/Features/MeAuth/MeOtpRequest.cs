namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeOtpRequest.</summary>
/// <param name="Phone">Serbest biçimli telefon.</param>
internal sealed record MeOtpRequest(string? Phone);
