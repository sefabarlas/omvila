using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme CustomerAppointmentRow ("21 Eki 14:00 · Fön · Elif · Tamamlandı · 450 ₺").</summary>
internal sealed record CustomerAppointmentRow(
    Guid AppointmentId,
    Guid BranchId,
    Instant StartUtc,
    string Status,
    string DisplayStatus,
    IReadOnlyList<string> ServiceNames,
    IReadOnlyList<string> StaffNames,
    Money Total);
