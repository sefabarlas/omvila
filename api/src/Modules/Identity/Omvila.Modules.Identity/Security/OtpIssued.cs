namespace Omvila.Modules.Identity.Security;

/// <summary>Sözleşme OtpChallengeResponse karşılığı.</summary>
internal sealed record OtpIssued(Guid OtpId, int ExpiresInSeconds, int ResendAfterSeconds, string PhoneMasked);
