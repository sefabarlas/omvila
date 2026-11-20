using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kalemin göreli planı (musaitlik.md § Girdi: ItemPlan): BufferBefore + aşamalar + BufferAfter. Tamponlarda personel
/// meşgul sayılır (alan-modeli.md § Service). Süresi sıfır olan tampon dilim üretmez. Müsaitlik motoru aday başlangıçları
/// bu şablonla dener; <see cref="At"/> şablonu bir başlangıca bağlar.
/// </summary>
internal sealed record ItemTemplate(ItemOffer Offer, IReadOnlyList<SegmentTemplate> Segments)
{
    /// <summary>İlk dilimin başından son dilimin sonuna toplam süre.</summary>
    public Duration TotalLength => Segments.Count == 0 ? Duration.Zero : Segments[^1].Offset + Segments[^1].Length;

    /// <summary>
    /// Son personel-meşgul dilimin bitişi (göreli). Çok kalemli randevuda sonraki kalem buradan başlar
    /// (musaitlik.md § Çok kalemli randevu). Personel-meşgul dilim yoksa toplam süre.
    /// </summary>
    public Duration StaffBusyEnd => Segments.LastOrDefault(s => s.StaffBusy) is { } last ? last.Offset + last.Length : TotalLength;

    public static ItemTemplate From(ItemOffer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);
        var segments = new List<SegmentTemplate>();
        var offset = Duration.Zero;

        void Add(SegmentKind kind, int minutes, bool staffBusy)
        {
            if (minutes <= 0)
            {
                return;
            }

            var length = Duration.FromMinutes(minutes);
            segments.Add(new SegmentTemplate(segments.Count + 1, kind, offset, length, staffBusy));
            offset += length;
        }

        Add(SegmentKind.BufferBefore, offer.BufferBeforeMinutes, staffBusy: true);
        foreach (var stage in offer.Stages)
        {
            Add(SegmentKind.Stage, stage.DurationMinutes, stage.StaffBusy);
        }

        Add(SegmentKind.BufferAfter, offer.BufferAfterMinutes, staffBusy: true);
        if (!segments.Exists(s => s.Kind == SegmentKind.Stage))
        {
            throw new DomainException("Hizmetin en az bir aşaması olmalı (aşamasız hizmette tek örtük aşama).");
        }

        return new ItemTemplate(offer, segments);
    }

    public ItemPlan At(int seqNo, Instant start) => new(
        seqNo,
        Offer,
        [.. Segments.Select(s => new PlannedSegment(s.SeqNo, s.Kind, new Interval(start + s.Offset, start + s.Offset + s.Length), s.StaffBusy))]);
}
