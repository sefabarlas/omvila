using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Sistem rollerinin izin matrisi: docs/alan/roller-yetkiler.md § İzin kataloğu ve matris ile birebir
/// (RolePermissionsDocumentTests doğrular). v1'de roller sabittir; markaya özel rol gelince veritabanına taşınır.
/// Personelin kendi randevusunu iptal etmesi ve kapora işaretlemesi v1'de kapalıdır (S-07).
/// </summary>
internal static class RolePermissions
{
    private const PermissionScope All = PermissionScope.All;
    private const PermissionScope Branch = PermissionScope.Branch;
    private const PermissionScope Own = PermissionScope.Own;

    // (izin, Owner, BranchManager, Staff); null = izin yok.
    private static readonly (string Permission, PermissionScope? Owner, PermissionScope? Manager, PermissionScope? Staff)[] Matrix =
    [
        (Permissions.Calendar.View, All, Branch, Own),
        (Permissions.Calendar.ViewAllStaff, All, Branch, null),
        (Permissions.Appointment.Create, All, Branch, Own),
        (Permissions.Appointment.Update, All, Branch, Own),
        (Permissions.Appointment.Cancel, All, Branch, null),
        (Permissions.Appointment.CancelCheckedIn, All, Branch, null),
        (Permissions.Appointment.Approve, All, Branch, null),
        (Permissions.Appointment.DepositMark, All, Branch, null),
        (Permissions.Appointment.CheckIn, All, Branch, Own),
        (Permissions.Appointment.Complete, All, Branch, null),
        (Permissions.Customer.View, All, All, All),
        (Permissions.Customer.PhoneView, All, All, null),
        (Permissions.Customer.Edit, All, All, All),
        (Permissions.Customer.Merge, All, All, null),
        (Permissions.Customer.Import, All, null, null),
        (Permissions.Customer.Export, All, null, null),
        (Permissions.Customer.Kvkk, All, null, null),
        (Permissions.WalkIn.Manage, All, Branch, All),
        (Permissions.Waitlist.Manage, All, Branch, null),
        (Permissions.Payment.Collect, All, Branch, null),
        (Permissions.Cash.Close, All, Branch, null),
        (Permissions.Package.Sell, All, Branch, null),
        (Permissions.Report.View, All, Branch, null),
        (Permissions.Commission.View, All, Branch, Own),
        (Permissions.Review.View, All, Branch, null),
        (Permissions.Review.Reply, All, Branch, null),
        // Yönetici yalnız kendi şubesinin fiyat ve uygunluğunu düzenler (belgedeki ***).
        (Permissions.Service.Manage, All, Branch, null),
        (Permissions.Package.Manage, All, null, null),
        (Permissions.Staff.Manage, All, Branch, null),
        (Permissions.TimeOff.Request, All, All, Own),
        (Permissions.Settings.Manage, All, null, null),
        (Permissions.Settings.Payment, All, null, null),
        (Permissions.Settings.Roles, All, null, null),
        (Permissions.Subscription.Manage, All, null, null),
    ];

    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, PermissionScope>> ByRole { get; } = Build();

    public static IEnumerable<string> AllPermissions => Matrix.Select(row => row.Permission);

    private static Dictionary<string, IReadOnlyDictionary<string, PermissionScope>> Build()
    {
        Dictionary<string, PermissionScope> Column(Func<(string, PermissionScope?, PermissionScope?, PermissionScope?), PermissionScope?> pick) =>
            Matrix.Where(row => pick(row) is not null).ToDictionary(row => row.Permission, row => pick(row)!.Value, StringComparer.Ordinal);

        return new Dictionary<string, IReadOnlyDictionary<string, PermissionScope>>(StringComparer.Ordinal)
        {
            [Roles.Owner] = Column(row => row.Item2),
            [Roles.BranchManager] = Column(row => row.Item3),
            [Roles.Staff] = Column(row => row.Item4),
        };
    }
}
