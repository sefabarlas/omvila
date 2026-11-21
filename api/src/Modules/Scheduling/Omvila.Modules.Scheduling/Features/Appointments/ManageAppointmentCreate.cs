using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme ManageAppointmentCreate. Biçim denetimi uçta; alanlar eksik gelebilir.</summary>
internal sealed record ManageAppointmentCreate(
    Guid? BranchId,
    Guid? BrandCustomerId,
    ManageAppointmentNewCustomer? NewCustomer,
    string? Channel,
    Instant? StartUtc,
    IReadOnlyList<AppointmentItemRequest>? Items,
    string? StaffNote,
    RecurrenceRequest? Recurrence,
    bool? NotifyCustomer,
    bool? AllowOutsideWorkingHours);
