using Omvila.Modules.Identity.Features.ManageAuth;

namespace Omvila.Modules.Identity.Features.PublicBookingAuth;

/// <summary>Sözleşme PublicOtpRequestResult: CodeSent (challenge) veya AlreadyVerified (booking).</summary>
internal sealed record PublicOtpRequestResult(string Status, OtpChallengeResponse? Challenge = null, BookingTokenResult? Booking = null);
