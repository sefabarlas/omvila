using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>Bildirim için yüklenmiş randevu: güncel randevu, salon/şube kartı, müşteri kartı ve şube saat dilimi.</summary>
internal sealed record AppointmentSmsContext(
    Guid BrandId,
    AppointmentNotificationInfo Appointment,
    PublicBranchCard Branch,
    CustomerDisplayInfo? Customer,
    DateTimeZone Zone)
{
    public bool IsConfirmed => string.Equals(Appointment.Status, "Confirmed", StringComparison.Ordinal);

    public bool IsPendingApproval => string.Equals(Appointment.Status, "PendingApproval", StringComparison.Ordinal);
}
