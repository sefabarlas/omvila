using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Platform.Contracts;

namespace Omvila.Modules.Customers.Features;

/// <summary>Denetim kaydının "kim, nereden" bilgisi (alan-modeli.md § AuditLog): jetondaki kullanıcı, istemci IP ve istemci adı.</summary>
internal sealed record AuditActor(Guid? UserId, string? IpAddress, string? UserAgent)
{
    public static AuditActor From(HttpContext http)
    {
        ArgumentNullException.ThrowIfNull(http);
        var userId = Guid.TryParse(http.User.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : (Guid?)null;
        return new AuditActor(userId, http.Connection.RemoteIpAddress?.ToString(), http.Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null);
    }

    public AuditEntry Entry(Guid brandId, AuditAction action, string entityType, Guid? entityId, IReadOnlyDictionary<string, object?>? changes) =>
        new(brandId, AuditActorType.User, UserId, action, entityType, entityId, changes, IpAddress, UserAgent);
}
