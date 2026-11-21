namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentItemRequest; staffId null = fark etmez.</summary>
internal sealed record AppointmentItemRequest(Guid? ServiceId, Guid? StaffId);
