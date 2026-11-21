namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>İstenen kalem (sözleşme AppointmentItemRequest); <see cref="StaffId"/> null = fark etmez.</summary>
internal sealed record RequestedItem(Guid ServiceId, Guid? StaffId = null);
