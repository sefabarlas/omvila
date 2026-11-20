namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Sözleşme SignupRequest.</summary>
internal sealed record SignupRequest(
    string? SignupToken, string? BusinessName, string? IndustryCode, string? OwnerFullName, bool AcceptedTerms, string? PrivacyNoticeVersion);
