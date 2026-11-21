using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentDetail (randevu yan paneli). Kapora (deposit) Dilim 4'te; şimdilik yazılmaz.</summary>
internal sealed record AppointmentDetail(
    Guid Id,
    Guid BranchId,
    string Status,
    string DisplayStatus,
    string Channel,
    Instant StartUtc,
    Instant EndUtc,
    string TimeZone,
    Instant? HoldExpiresUtc,
    AppointmentCustomerView Customer,
    IReadOnlyList<AppointmentItemView> Items,
    Money Total,
    Money AmountDue,
    string? CustomerNote,
    string? StaffNote,
    bool IsRecurring,
    Guid? SeriesId,
    Instant? NoShowAtUtc,
    AppointmentCancellationView? Cancellation,
    IReadOnlyList<AppointmentHistoryEntry> History,
    string Version);
