using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Platform rollerinin izin matrisi: docs/alan/roller-yetkiler.md § Platform rolleri ile birebir (PlatformRolePermissionsDocumentTests
/// doğrular). Kapsam yoktur: platform izni bütün markalar içindir; marka verisi yalnız görüntülenir.
/// </summary>
internal static class PlatformRolePermissions
{
    // (izin, PlatformAdmin, PlatformSupport, PlatformFinance).
    private static readonly (string Permission, bool Admin, bool Support, bool Finance)[] Matrix =
    [
        (AdminPermissions.BrandsView, true, true, true),
        (AdminPermissions.PaymentsMatch, true, true, true),
        (AdminPermissions.SmsSendersDecide, true, true, false),
        (AdminPermissions.CreditsGrant, true, false, false),
        (AdminPermissions.BrandsManage, true, false, false),
        (AdminPermissions.PlansView, true, true, true),
        (AdminPermissions.PlansManage, true, false, true),
        (AdminPermissions.BillingManage, true, false, true),
        (AdminPermissions.AuditView, true, true, true),
        (AdminPermissions.AuditExport, true, false, false),
    ];

    public static IReadOnlyDictionary<string, IReadOnlySet<string>> ByRole { get; } = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
    {
        [PlatformRoles.Admin] = Matrix.Where(r => r.Admin).Select(r => r.Permission).ToHashSet(StringComparer.Ordinal),
        [PlatformRoles.Support] = Matrix.Where(r => r.Support).Select(r => r.Permission).ToHashSet(StringComparer.Ordinal),
        [PlatformRoles.Finance] = Matrix.Where(r => r.Finance).Select(r => r.Permission).ToHashSet(StringComparer.Ordinal),
    };

    public static IReadOnlyList<string> AllPermissions { get; } = [.. Matrix.Select(r => r.Permission)];

    /// <summary>Rolün izinleri, matris sırasıyla (adminGetSession).</summary>
    public static IReadOnlyList<string> For(string role) =>
        ByRole.TryGetValue(role, out var permissions) ? [.. AllPermissions.Where(permissions.Contains)] : [];
}
