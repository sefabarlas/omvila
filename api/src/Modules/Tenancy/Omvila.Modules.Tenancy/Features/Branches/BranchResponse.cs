using System.Globalization;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme Branch.</summary>
internal sealed record BranchResponse(
    Guid Id,
    string Name,
    string Slug,
    string AddressLine,
    string District,
    string City,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    string? Phone,
    string? Email,
    string TimeZone,
    int? SlotIntervalMinutes,
    bool IsActive,
    string Version)
{
    public static BranchResponse From(Domain.Branch b) => new(
        b.Id, b.Name, b.Slug, b.AddressLine, b.District, b.City, b.PostalCode, b.Latitude, b.Longitude, b.Phone, b.Email,
        b.TimeZoneId, b.SlotIntervalMinutes, b.IsActive, b.Version.ToString(CultureInfo.InvariantCulture));
}
