using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>AppointmentDetail.customer. <see cref="Phone"/> yalnız tam numara gösterilebiliyorsa yazılır (T-46).</summary>
internal sealed record AppointmentCustomerView(
    Guid BrandCustomerId, string FullName, string? Phone, string? PhoneMasked, int VisitCount, LocalDate? LastVisitOn, bool IsNew);
