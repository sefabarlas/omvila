namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>Personelin haftalık doluluğu (satırdaki nokta takvim rengidir).</summary>
internal sealed record StaffOccupancy(Guid StaffId, string StaffName, string CalendarColor, int Percent);
