using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>Sözleşme CalendarAppointment: takvim bloğu (bir kalem = bir blok). staffId zorunlu ve null olabilir.</summary>
internal sealed record CalendarAppointment(
    Guid AppointmentId,
    Guid ItemId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? StaffId,
    Instant StartUtc,
    Instant EndUtc,
    string Status,
    string DisplayStatus,
    string CustomerName,
    string ServiceName,
    bool IsNewCustomer,
    bool IsRecurring,
    Instant? HoldExpiresUtc,
    IReadOnlyList<SegmentView> Segments);
