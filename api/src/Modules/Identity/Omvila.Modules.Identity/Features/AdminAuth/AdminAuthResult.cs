namespace Omvila.Modules.Identity.Features.AdminAuth;

/// <summary>Sözleşme AdminAuthResult (jetonlar gövdede; admin yüzeyi çerez kullanmaz).</summary>
internal sealed record AdminAuthResult(string AccessToken, int ExpiresInSeconds, string RefreshToken);
