using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Hizmet ekleme ve düzenleme girdisi (sözleşme ServiceWrite'ın alan karşılığı).</summary>
internal sealed record ServiceDraft(
    string Name,
    int DurationMinutes,
    Money Price,
    PriceDisplay PriceDisplay,
    int BufferBeforeMinutes,
    int BufferAfterMinutes,
    DepositRule Deposit,
    IReadOnlyList<ServiceStage> Stages);
