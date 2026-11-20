namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Sözleşme şeması (contracts/openapi.yaml § manage-auth).</summary>
internal sealed record RefreshRequest(string? RefreshToken);
