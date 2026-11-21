using NodaTime;

namespace Omvila.Modules.Identity.Security;

/// <summary>Yeni güvenilir cihaz: çereze yazılacak açık değer ve bitiş.</summary>
internal sealed record IssuedTrustedDevice(string Token, Instant ExpiresUtc);
