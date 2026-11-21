using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Yazılan randevu. <see cref="AccessCode"/> yalnız oluşturmada doludur (SMS bağlantısı). <see cref="AcceptedWarnings"/>
/// panelde onayla geçilen uyarılardır (çalışma saati dışı, izin).
/// </summary>
internal sealed record AppointmentWriteResult(Appointment Appointment, AppointmentAccessCode? AccessCode, SlotViolation AcceptedWarnings);
