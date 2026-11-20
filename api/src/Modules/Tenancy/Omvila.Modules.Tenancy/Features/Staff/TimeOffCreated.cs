namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme TimeOffCreated. Etkilenen randevular Dilim 2'de (CalendarAppointment) dolar.</summary>
internal sealed record TimeOffCreated(TimeOffResponse TimeOff, IReadOnlyList<object> AffectedAppointments);
