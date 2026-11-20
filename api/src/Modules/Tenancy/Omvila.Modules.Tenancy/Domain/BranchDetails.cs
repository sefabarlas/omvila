namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Şubenin düzenlenebilir alanları (sözleşme BranchCreate/BranchUpdate'in uygulanmış hâli; telefon E.164).</summary>
internal sealed record BranchDetails(
    string Name,
    string AddressLine,
    string District,
    string City,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    string? Phone,
    string? Email,
    int? SlotIntervalMinutes,
    bool IsActive);
