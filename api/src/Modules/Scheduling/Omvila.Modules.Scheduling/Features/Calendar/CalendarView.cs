using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>Sözleşme CalendarView (günlük görünüm: personel kolonları; haftalık: tek personel).</summary>
internal sealed record CalendarView(
    Guid BranchId,
    string TimeZone,
    LocalDate From,
    LocalDate To,
    IReadOnlyList<CalendarStaff> Staff,
    IReadOnlyList<CalendarDay> Days,
    IReadOnlyList<CalendarAppointment> Appointments,
    IReadOnlyList<TimeOff> TimeOffs);
