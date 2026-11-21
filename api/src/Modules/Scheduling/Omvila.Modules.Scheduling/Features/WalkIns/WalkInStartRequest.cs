namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>Sözleşme WalkInStartRequest; verilmeyen alan biletteki değerdir.</summary>
internal sealed record WalkInStartRequest(Guid? StaffId, Guid? ServiceId);
