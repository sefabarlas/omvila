using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme CustomerSummary. <see cref="Phone"/> yalnız tam numara gösterilebiliyorsa yazılır.</summary>
internal sealed record CustomerSummaryResponse(
    Guid Id,
    string FullName,
    string? Phone,
    string? PhoneMasked,
    int? BirthYear,
    int VisitCount,
    int NoShowCount,
    LocalDate? LastVisitOn,
    Instant? NextAppointmentUtc,
    Money TotalSpent,
    bool IsNew,
    bool IsNoShowRisk,
    IReadOnlyList<string> Tags,
    string? Notes);
