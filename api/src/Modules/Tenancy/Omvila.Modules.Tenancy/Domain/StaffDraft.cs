namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Personel ekleme girdisi (sözleşme StaffCreate'in alan karşılığı).</summary>
internal sealed record StaffDraft(
    string FullName,
    string? PhoneInput,
    StaffRole Role,
    string CalendarColor,
    IReadOnlyCollection<Guid> BranchIds,
    decimal ServiceCommissionPercent,
    decimal SaleCommissionPercent,
    bool SendInvite);
