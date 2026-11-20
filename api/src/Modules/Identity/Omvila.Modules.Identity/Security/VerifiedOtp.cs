using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Security;

internal sealed record VerifiedOtp(string PhoneE164, OtpPurpose Purpose);
