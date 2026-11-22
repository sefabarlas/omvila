using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Features.Users;

/// <summary>
/// AyarlarRoller "Rol izinleri" (sözleşme manageGetRoleMatrix, K6): ekran satırları izin kataloğunun grupları, değerler
/// <see cref="RolePermissions"/>'tan (belgeyle eşli). Bir rolün satırdaki düzeyi satırın izinlerinden en genişidir
/// (All &gt; Branch &gt; Own &gt; None): ör. "Raporlar ve prim"de personel raporu görmez ama primini görür, "Kendi" yazılır.
/// </summary>
internal static class RoleMatrix
{
    // Ekrandaki sıra (AyarlarRoller tasarımı).
    internal static readonly (string Group, string[] Permissions)[] Groups =
    [
        ("CalendarAndAppointments", [Permissions.Calendar.View, Permissions.Appointment.Create, Permissions.Appointment.Update]),
        ("CustomerPhone", [Permissions.Customer.PhoneView]),
        ("CustomerCardAndNotes", [Permissions.Customer.View, Permissions.Customer.Edit]),
        ("CashAndPayments", [Permissions.Payment.Collect, Permissions.Cash.Close]),
        ("ReportsAndCommission", [Permissions.Report.View, Permissions.Commission.View]),
        ("ServicesAndPrices", [Permissions.Service.Manage]),
        ("Settings", [Permissions.Settings.Manage, Permissions.Settings.Payment, Permissions.Settings.Roles]),
    ];

    public static RoleMatrixResponse Build() => new(
        [.. Groups.Select(g => new RoleMatrixRow(
            g.Group, g.Permissions, Level(Roles.Owner, g.Permissions), Level(Roles.BranchManager, g.Permissions), Level(Roles.Staff, g.Permissions)))]);

    internal static string Level(string role, IReadOnlyCollection<string> permissions)
    {
        var granted = RolePermissions.ByRole[role];
        var scopes = permissions.Where(granted.ContainsKey).Select(p => granted[p]).ToList();
        if (scopes.Count == 0)
        {
            return "None";
        }

        return scopes.Contains(PermissionScope.All) ? "All" : scopes.Contains(PermissionScope.Branch) ? "Branch" : "Own";
    }
}
