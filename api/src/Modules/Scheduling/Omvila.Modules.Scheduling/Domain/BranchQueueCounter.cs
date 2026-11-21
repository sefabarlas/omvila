using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kapıda sıra numaratörü (alan-modeli.md § WalkInTicket: BranchQueueCounter). Satır şube + yerel gün başına tektir; numara
/// <c>BranchQueueNumbers</c> ile tek SQL'de atomik verilir (INSERT … ON CONFLICT DO UPDATE … RETURNING), uygulama yazmaz.
/// </summary>
internal sealed class BranchQueueCounter
{
    private BranchQueueCounter()
    {
    }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public LocalDate QueueDate { get; private set; }

    public int LastNumber { get; private set; }
}
