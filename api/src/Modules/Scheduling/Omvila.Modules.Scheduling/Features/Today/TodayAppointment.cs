using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>Sözleşme TodayAppointment ("12:00 · Deniz Acar · Saç kesimi · Can · Onay bekliyor"); telefon taşımaz.</summary>
internal sealed record TodayAppointment(
    Guid AppointmentId,
    Instant StartUtc,
    Instant EndUtc,
    string Status,
    string DisplayStatus,
    string CustomerName,
    bool IsNewCustomer,
    string ServiceSummary,
    IReadOnlyList<string> StaffNames,
    Instant? HoldExpiresUtc,
    string? DepositStatus);
