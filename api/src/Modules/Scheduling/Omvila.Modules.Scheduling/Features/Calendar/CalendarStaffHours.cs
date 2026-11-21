namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>CalendarView.days.staffHours.</summary>
internal sealed record CalendarStaffHours(Guid StaffId, IReadOnlyList<TimeIntervalView> Intervals);
