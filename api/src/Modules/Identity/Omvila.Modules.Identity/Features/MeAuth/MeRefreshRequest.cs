namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeRefreshRequest.</summary>
/// <param name="RefreshToken">Saklanan yenileme jetonu.</param>
internal sealed record MeRefreshRequest(string? RefreshToken);
