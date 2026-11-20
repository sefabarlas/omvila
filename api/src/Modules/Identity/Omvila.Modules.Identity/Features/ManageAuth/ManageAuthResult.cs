namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Sözleşme şeması (contracts/openapi.yaml § manage-auth).</summary>
internal sealed record ManageAuthResult(string Result, string? AccessToken = null, int? ExpiresInSeconds = null, string? RefreshToken = null, string? SignupToken = null);
