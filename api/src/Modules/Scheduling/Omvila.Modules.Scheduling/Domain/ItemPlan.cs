using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Bir başlangıca bağlanmış kalem planı.</summary>
internal sealed record ItemPlan(int SeqNo, ItemOffer Offer, IReadOnlyList<PlannedSegment> Segments)
{
    /// <summary>Kalemin başlangıcı: ilk dilimin (varsa önce tamponun) başı.</summary>
    public Instant StartUtc => Segments[0].During.Start;

    /// <summary>Kalemin bitişi: son dilimin (varsa sonra tamponun) sonu.</summary>
    public Instant EndUtc => Segments[^1].During.End;

    /// <summary>Son personel-meşgul dilimin bitişi; sonraki kalemin başlangıcı.</summary>
    public Instant StaffBusyEndUtc => Segments.LastOrDefault(s => s.StaffBusy)?.During.End ?? EndUtc;
}
