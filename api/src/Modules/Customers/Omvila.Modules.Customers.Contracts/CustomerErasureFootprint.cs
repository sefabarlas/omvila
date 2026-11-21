namespace Omvila.Modules.Customers.Contracts;

/// <summary>Bir modülün silme önizlemesine katkısı; katılımcıların toplamı yanıttır.</summary>
/// <param name="Appointments">Anonim korunacak randevular.</param>
/// <param name="Payments">Anonim korunacak ödemeler.</param>
/// <param name="UpcomingAppointments">İptal edilecek yaklaşan randevular.</param>
/// <param name="ActivePackages">Aktif paketler (paket modülü geldiğinde).</param>
public sealed record CustomerErasureFootprint(
    int Appointments = 0, int Payments = 0, int UpcomingAppointments = 0, IReadOnlyList<ActivePackageInfo>? ActivePackages = null)
{
    /// <summary>Katkısı olmayan modül.</summary>
    public static readonly CustomerErasureFootprint None = new();

    /// <summary>İki katkının toplamı.</summary>
    /// <param name="other">Diğer katkı.</param>
    /// <returns>Toplam.</returns>
    public CustomerErasureFootprint Add(CustomerErasureFootprint other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return new CustomerErasureFootprint(
            Appointments + other.Appointments,
            Payments + other.Payments,
            UpcomingAppointments + other.UpcomingAppointments,
            [.. ActivePackages ?? [], .. other.ActivePackages ?? []]);
    }
}
