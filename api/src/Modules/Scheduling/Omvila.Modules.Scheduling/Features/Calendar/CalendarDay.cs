using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>CalendarView.days: şubenin o günkü saatleri ve personelin çalışma saatleri. Kapalı gün "Kapalı gün" boş hâlidir.</summary>
internal sealed record CalendarDay(
    LocalDate Date, bool IsClosed, string? ClosureNote, IReadOnlyList<TimeIntervalView> BranchIntervals, IReadOnlyList<CalendarStaffHours> StaffHours);
