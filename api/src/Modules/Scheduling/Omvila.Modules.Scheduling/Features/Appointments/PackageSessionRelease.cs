using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// İptalde paket seansının geri dönüşü (durum-makinesi.md A4–A7: "seans geri"). Her iptal yolu (salon iptali, red, müşteri
/// iptali, süre dolumu ve yazma yolundaki satır içi süre dolumu) randevuyu kaydettikten sonra aynı transaction'da çağırır;
/// gelmedi (A8) çağırmaz, seans geri dönmez (S-09).
/// </summary>
internal static class PackageSessionRelease
{
    public static Task ReleaseAsync(this IPackageSessions sessions, Guid brandId, IEnumerable<Appointment> cancelled, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        var ids = cancelled.Where(a => a.Status == AppointmentStatus.Cancelled && a.UsesPackage).Select(a => a.Id).ToList();
        return ids.Count == 0 ? Task.CompletedTask : sessions.RevertAsync(brandId, ids, ct);
    }
}
