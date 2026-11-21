using NodaTime;
using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>Sözleşme DaySearchRequest.</summary>
internal sealed record DaySearchRequest(IReadOnlyList<AppointmentItemRequest>? Items, bool? SameStaffForAll, LocalDate? From, LocalDate? To);
