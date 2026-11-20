using NodaTime;

namespace Omvila.Modules.Identity.Security;

/// <summary>Yeni üretilen yenileme jetonu: düz değer yalnız istemciye gider, saklanmaz.</summary>
internal sealed record IssuedRefreshToken(string Token, Instant ExpiresUtc, Guid UserId, Guid? MembershipId, bool RememberDevice);
