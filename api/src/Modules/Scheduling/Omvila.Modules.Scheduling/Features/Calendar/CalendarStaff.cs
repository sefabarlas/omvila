namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>CalendarView.staff: personel kolonu.</summary>
internal sealed record CalendarStaff(Guid StaffId, string FullName, string CalendarColor, string? PhotoUrl, int AppointmentCount, bool HasGapAvailability);
