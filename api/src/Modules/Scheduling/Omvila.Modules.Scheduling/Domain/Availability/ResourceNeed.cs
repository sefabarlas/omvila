namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Hizmetin kaynak ihtiyacı (alan-modeli.md § ServiceResourceRule): türünden <see cref="Quantity"/> birim, aşama
/// aralığında (<see cref="StageSeqFrom"/>–<see cref="StageSeqTo"/>, aşama sırası 1'den). İkisi de null ise kalemin
/// tamamı (tamponlar dahil; ihtiyatlı yorum, bkz. T-48).
/// </summary>
internal sealed record ResourceNeed(Guid ResourceTypeId, int Quantity, int? StageSeqFrom = null, int? StageSeqTo = null);
