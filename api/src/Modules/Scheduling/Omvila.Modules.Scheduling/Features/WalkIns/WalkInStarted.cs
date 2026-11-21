using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>Sözleşme WalkInStarted.</summary>
internal sealed record WalkInStarted(WalkInTicketView Ticket, AppointmentDetail Appointment);
