using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.DemoData;

/// <summary>
/// Demo hizmeti. <paramref name="StartingFrom"/>: "1.800 ₺'den". <paramref name="RequiresApproval"/> null: marka ayarı geçerli.
/// </summary>
public sealed record DemoService(
    Guid CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    Money Price,
    bool StartingFrom,
    bool? RequiresApproval,
    bool IsOnlineBookable,
    IReadOnlyList<Guid> StaffIds);
