using NodaTime;

namespace Omvila.Modules.Tenancy.Features.PublicBrand;

/// <summary>PublicBrand.branches[].</summary>
internal sealed record PublicBranchResponse(
    Guid Id,
    string Name,
    string AddressLine,
    string District,
    string City,
    string? Phone,
    decimal? Latitude,
    decimal? Longitude,
    string TimeZone,
    bool OpenNow,
    string? TodayClosesAt,
    Instant? NextOpeningUtc);
