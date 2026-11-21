using NodaTime;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>Sözleşme WalkInQueue.</summary>
internal sealed record WalkInQueue(
    Guid BranchId,
    LocalDate Date,
    string TimeZone,
    Instant GeneratedUtc,
    IReadOnlyList<WalkInTicketView> Items,
    int WaitingCount,
    Instant? OldestJoinedUtc,
    int? EstimatedWaitMinutesForNew,
    int ServedCount,
    int NextQueueNumber);
