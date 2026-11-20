namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Sözleşme şeması (contracts/openapi.yaml § manage-auth).</summary>
internal sealed record OtpVerifyRequest(Guid OtpId, string? Code, bool RememberDevice = false);
