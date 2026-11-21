using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme ManageRescheduleRequest.</summary>
internal sealed record ManageRescheduleRequest(
    Instant? StartUtc, IReadOnlyList<ManageRescheduleItem>? Items, string? Scope, bool? NotifyCustomer, bool? AllowOutsideWorkingHours);
