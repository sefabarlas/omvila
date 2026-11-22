namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeLogoutRequest.</summary>
/// <param name="RefreshToken">Zinciri iptal edilecek yenileme jetonu.</param>
/// <param name="PushToken">Bu telefonun push jetonu; cihaz kaydı pasifleşir.</param>
internal sealed record MeLogoutRequest(string? RefreshToken, string? PushToken);
