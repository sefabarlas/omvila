namespace Omvila.Infrastructure.Security;

/// <summary>Platform rolü + izin → var mı (roller-yetkiler.md § Platform rolleri). Uygulaması Identity modülündedir.</summary>
public interface IAdminPermissionResolver
{
    bool HasPermission(string platformRole, string permission);
}
