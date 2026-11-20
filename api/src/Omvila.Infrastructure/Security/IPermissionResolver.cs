namespace Omvila.Infrastructure.Security;

/// <summary>Rol + izin → kapsam (izin yoksa null). Uygulaması Identity modülündedir.</summary>
public interface IPermissionResolver
{
    PermissionScope? Resolve(string role, string permission);
}
