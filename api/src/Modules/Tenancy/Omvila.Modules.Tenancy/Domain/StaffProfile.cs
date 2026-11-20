namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Personelin düzenlenebilir alanları (sözleşme StaffUpdate'in uygulanmış hâli).</summary>
internal sealed record StaffProfile(
    string FullName,
    string? DisplayName,
    string? Title,
    string? Bio,
    string CalendarColor,
    decimal ServiceCommissionPercent,
    decimal SaleCommissionPercent,
    bool IsBookableOnline,
    bool IsActive);
