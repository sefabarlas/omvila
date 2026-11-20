using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// displayStatus hesabı (durum-makinesi.md § Arayüz gösterimi; alan-modeli.md § Appointment "Kapora bekleniyor bir durum
/// değildir"): PendingApproval + kapora istenmiş + alınmamış = DepositPending; diğerleri durumla aynı adı taşır.
/// </summary>
internal static class DisplayStatusRule
{
    public static DisplayStatus For(AppointmentStatus status, long? depositRequiredMinor, Instant? depositReceivedUtc) => status switch
    {
        AppointmentStatus.PendingApproval when depositRequiredMinor is not null && depositReceivedUtc is null => DisplayStatus.DepositPending,
        AppointmentStatus.PendingApproval => DisplayStatus.PendingApproval,
        AppointmentStatus.Confirmed => DisplayStatus.Confirmed,
        AppointmentStatus.CheckedIn => DisplayStatus.CheckedIn,
        AppointmentStatus.Completed => DisplayStatus.Completed,
        AppointmentStatus.Cancelled => DisplayStatus.Cancelled,
        AppointmentStatus.NoShow => DisplayStatus.NoShow,
        // Draft v1'de üretilmez ve hiçbir ekranda görünmez (durum-makinesi.md § 1).
        _ => throw new DomainException($"{status} için gösterim yok."),
    };
}
