using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.PublicCatalog;

/// <summary>Sözleşme PublicCatalog.</summary>
internal sealed record PublicCatalogResponse(
    Guid BranchId,
    string TimeZone,
    bool AllowStaffSelection,
    bool AllowAnyStaffOption,
    string? PriceNote,
    IReadOnlyList<PublicCatalogResponse.Category> Categories,
    IReadOnlyList<PublicCatalogResponse.ServiceItem> Services,
    IReadOnlyList<PublicCatalogResponse.StaffItem> Staff)
{
    /// <summary>PublicCatalog.categories[].</summary>
    internal sealed record Category(Guid Id, string Name);

    /// <summary>PublicCatalog.services[]; <see cref="Price"/> fiyatlar gizliyse null.</summary>
    internal sealed record ServiceItem(
        Guid Id,
        Guid CategoryId,
        string Name,
        string? Description,
        int DurationMinutes,
        Money? Price,
        string PriceDisplay,
        bool HasDeposit,
        Money? DepositHint,
        IReadOnlyList<Guid> StaffIds);

    /// <summary>PublicCatalog.staff[].</summary>
    internal sealed record StaffItem(Guid Id, string DisplayName, string? Title, string? PhotoUrl, IReadOnlyList<Surcharge> Surcharges);

    /// <summary>PublicCatalog.staff[].surcharges[] ("+50 ₺").</summary>
    internal sealed record Surcharge(Guid ServiceId, Money Amount);
}
