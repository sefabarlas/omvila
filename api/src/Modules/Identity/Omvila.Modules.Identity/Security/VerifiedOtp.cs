using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Security;

/// <summary>Doğrulanmış kod: telefon, amaç, kod kimliği ve (rezervasyonda) kodun istendiği şube.</summary>
internal sealed record VerifiedOtp(string PhoneE164, OtpPurpose Purpose, Guid OtpId, Guid? BranchId);
