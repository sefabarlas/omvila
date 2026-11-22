using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Platform.Contracts;

namespace Omvila.Modules.Platform.Features;

/// <summary>Admin isteğini yapan platform kullanıcısı ve denetim kaydı girdisi (IP, istemci).</summary>
internal static class AdminActor
{
    /// <summary>Jetondaki platform kullanıcısı (admin uçları kimlik doğrulamalı olduğundan her zaman vardır).</summary>
    public static Guid UserId(HttpContext http)
    {
        ArgumentNullException.ThrowIfNull(http);
        return Guid.TryParse(http.User.FindFirstValue(OmvilaClaims.Subject), out var id) ? id : Guid.Empty;
    }

    public static AuditEntry Entry(
        HttpContext http, Guid? brandId, AuditAction action, string entityType, Guid? entityId, IReadOnlyDictionary<string, object?> changes)
    {
        ArgumentNullException.ThrowIfNull(http);
        var actor = UserId(http);
        return new AuditEntry(
            brandId,
            AuditActorType.PlatformUser,
            actor == Guid.Empty ? null : actor,
            action,
            entityType,
            entityId,
            changes,
            http.Connection.RemoteIpAddress?.ToString(),
            http.Request.Headers.UserAgent.ToString());
    }

    public static Dictionary<string, object?> Changes() => new(StringComparer.Ordinal);
}
