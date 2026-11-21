using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentDetail (randevu yan paneli). <c>deposit</c> kapora istenmemişse yazılmaz; havale talimatı panelde yoktur.</summary>
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
    DepositView? Deposit,
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
