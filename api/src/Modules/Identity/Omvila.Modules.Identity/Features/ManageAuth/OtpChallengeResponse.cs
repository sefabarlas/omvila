namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Sözleşme şeması (contracts/openapi.yaml § manage-auth).</summary>
internal sealed record OtpChallengeResponse(Guid OtpId, int ExpiresInSeconds, int ResendAfterSeconds, string PhoneMasked);
