using NodaTime;

namespace Omvila.Modules.Scheduling.Features.AppointmentLink;

/// <summary>Sözleşme publicSearchRescheduleSlots gövdesi.</summary>
internal sealed record RescheduleSlotSearchRequest(LocalDate? Date);
