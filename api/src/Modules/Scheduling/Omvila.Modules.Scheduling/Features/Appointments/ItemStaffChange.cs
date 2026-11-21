namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Saat değişikliğinde kalemin yeni personeli (sözleşme ManageRescheduleRequest.items).</summary>
internal sealed record ItemStaffChange(Guid ItemId, Guid StaffId);
