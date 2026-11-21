using System.Security.Claims;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Features;

/// <summary>
/// Bir isteğin randevu kapsamı (roller-yetkiler.md § İzin kataloğu: Ş = kendi şubeleri, K = yalnız kendi kayıtları).
/// Personel rolünde "kendi" = randevu kaleminin StaffId'si jetondaki staff_id'dir. Kapsam dışı kayıt 404 ile yanıtlanır.
/// </summary>
/// <param name="Branches">Jetondaki şube kapsamı.</param>
/// <param name="OwnOnly">İzin yalnız kendi kayıtları kapsıyor (K).</param>
/// <param name="OwnStaffId">Jetondaki personel; üyelik personele bağlı değilse null (K kapsamında hiçbir kayıt görünmez).</param>
/// <param name="SeesAllStaff">calendar.view.all-staff (diğer personelin kolonları).</param>
/// <param name="HasPhoneView">customer.phone.view (marka maskelemiyorsa herkes tam numarayı görür; T-46).</param>
internal sealed record ManageAccess(BranchScope Branches, bool OwnOnly, Guid? OwnStaffId, bool SeesAllStaff, bool HasPhoneView)
{
    public static ManageAccess For(ClaimsPrincipal user, IPermissionResolver resolver, string permission)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(resolver);
        var role = user.FindFirst(OmvilaClaims.Role)?.Value;
        var scope = role is null ? null : resolver.Resolve(role, permission);
        var staffId = Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var id) ? id : (Guid?)null;
        var seesAll = role is not null && resolver.Resolve(role, Permissions.Calendar.ViewAllStaff) is not null;
        var phoneView = role is not null && resolver.Resolve(role, Permissions.Customer.PhoneView) is not null;
        return new ManageAccess(BranchScope.From(user), scope == PermissionScope.Own, staffId, seesAll, phoneView);
    }

    /// <summary>Personel K kapsamında bu personelin kaydı mı (K değilse her personel).</summary>
    public bool IsOwnStaff(Guid? staffId) => !OwnOnly || (staffId is not null && staffId == OwnStaffId);

    /// <summary>Randevu kapsamda mı: şube kapsamda ve (K ise) kalemlerinden biri kendi personelinin.</summary>
    public bool Covers(Appointment appointment)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        return Branches.Includes(appointment.BranchId) && (!OwnOnly || appointment.Items.Any(i => IsOwnStaff(i.StaffId)));
    }
}
