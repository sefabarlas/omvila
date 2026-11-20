namespace Omvila.Modules.Catalog.Domain;

/// <summary>Sözleşme ServiceWrite'ın doğrulanmış hâli: kural girdisi (<see cref="ServiceDraft"/>) ve diğer alanlar.</summary>
internal sealed record ServiceDetails(
    Guid CategoryId,
    ServiceDraft Draft,
    string? Description,
    bool? RequiresApproval,
    bool IsOnlineBookable,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays,
    int SortOrder,
    bool IsActive);
