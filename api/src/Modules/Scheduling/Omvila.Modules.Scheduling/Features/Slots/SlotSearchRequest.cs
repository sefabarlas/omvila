using NodaTime;
using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>Sözleşme SlotSearchRequest.</summary>
internal sealed record SlotSearchRequest(IReadOnlyList<AppointmentItemRequest>? Items, bool? SameStaffForAll, LocalDate? Date);
