using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Yarı açık <c>[Start, End)</c> <see cref="Instant"/> aralıklarının kümesi (musaitlik.md § Kavramlar: aralık cebri).
/// İç gösterim normaldir: aralıklar başlangıca göre sıralı, boş değil, örtüşmez ve bitişik değildir (14:00'te biten ile
/// 14:00'te başlayan tek aralıkta birleşir). Değişmezdir; her işlem yeni küme döner. Sonsuz aralık kabul edilmez.
/// </summary>
internal sealed class IntervalSet : IEquatable<IntervalSet>
{
    private readonly Interval[] _items;

    private IntervalSet(Interval[] items) => _items = items;

    public static IntervalSet Empty { get; } = new([]);

    public IReadOnlyList<Interval> Intervals => _items;

    public bool IsEmpty => _items.Length == 0;

    public Duration TotalDuration
    {
        get
        {
            var total = Duration.Zero;
            foreach (var item in _items)
            {
                total += item.Duration;
            }

            return total;
        }
    }

    public static IntervalSet Of(params IEnumerable<Interval> intervals) => From(intervals);

    /// <summary>Herhangi bir aralık dizisini normal kümeye çevirir (boşlar atılır, örtüşen ve bitişikler birleşir).</summary>
    public static IntervalSet From(IEnumerable<Interval> intervals)
    {
        ArgumentNullException.ThrowIfNull(intervals);
        var sorted = new List<Interval>();
        foreach (var interval in intervals)
        {
            if (!interval.HasStart || !interval.HasEnd)
            {
                throw new ArgumentException("Sonsuz aralık müsaitlik hesabında kullanılmaz.", nameof(intervals));
            }

            if (interval.Start < interval.End)
            {
                sorted.Add(interval);
            }
        }

        if (sorted.Count == 0)
        {
            return Empty;
        }

        sorted.Sort((a, b) => a.Start.CompareTo(b.Start));
        var merged = new List<Interval>(sorted.Count);
        var current = sorted[0];
        for (var i = 1; i < sorted.Count; i++)
        {
            var next = sorted[i];
            if (next.Start <= current.End)
            {
                current = new Interval(current.Start, Max(current.End, next.End));
            }
            else
            {
                merged.Add(current);
                current = next;
            }
        }

        merged.Add(current);
        return new IntervalSet([.. merged]);
    }

    public IntervalSet Union(IntervalSet other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other.IsEmpty)
        {
            return this;
        }

        return IsEmpty ? other : From(_items.Concat(other._items));
    }

    public IntervalSet Intersect(IntervalSet other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var result = new List<Interval>();
        int i = 0, j = 0;
        while (i < _items.Length && j < other._items.Length)
        {
            var a = _items[i];
            var b = other._items[j];
            var start = Max(a.Start, b.Start);
            var end = Min(a.End, b.End);
            if (start < end)
            {
                result.Add(new Interval(start, end));
            }

            if (a.End <= b.End)
            {
                i++;
            }
            else
            {
                j++;
            }
        }

        return result.Count == 0 ? Empty : new IntervalSet([.. result]);
    }

    public IntervalSet Intersect(Interval window) => Intersect(Of(window));

    public IntervalSet Subtract(IntervalSet other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (IsEmpty || other.IsEmpty)
        {
            return this;
        }

        var result = new List<Interval>();
        var j = 0;
        foreach (var item in _items)
        {
            var cursor = item.Start;
            while (j < other._items.Length && other._items[j].End <= cursor)
            {
                j++;
            }

            var k = j;
            while (k < other._items.Length && other._items[k].Start < item.End)
            {
                var cut = other._items[k];
                if (cut.Start > cursor)
                {
                    result.Add(new Interval(cursor, cut.Start));
                }

                cursor = Max(cursor, cut.End);
                if (cursor >= item.End)
                {
                    break;
                }

                k++;
            }

            if (cursor < item.End)
            {
                result.Add(new Interval(cursor, item.End));
            }
        }

        return result.Count == 0 ? Empty : new IntervalSet([.. result]);
    }

    /// <summary>Kümeyi bir anda ikiye böler: <c>Before</c> = an öncesi, <c>After</c> = an ve sonrası.</summary>
    public (IntervalSet Before, IntervalSet After) Split(Instant at)
    {
        var before = new List<Interval>();
        var after = new List<Interval>();
        foreach (var item in _items)
        {
            if (item.End <= at)
            {
                before.Add(item);
            }
            else if (item.Start >= at)
            {
                after.Add(item);
            }
            else
            {
                before.Add(new Interval(item.Start, at));
                after.Add(new Interval(at, item.End));
            }
        }

        return (before.Count == 0 ? Empty : new IntervalSet([.. before]), after.Count == 0 ? Empty : new IntervalSet([.. after]));
    }

    /// <summary>Aralığın tamamı kümede mi (boş aralık her zaman içerilir).</summary>
    public bool Contains(Interval interval)
    {
        if (interval.Start >= interval.End)
        {
            return true;
        }

        var index = LastStartingAtOrBefore(interval.Start);
        return index >= 0 && _items[index].End >= interval.End;
    }

    public bool Contains(Instant instant)
    {
        var index = LastStartingAtOrBefore(instant);
        return index >= 0 && _items[index].End > instant;
    }

    /// <summary>Aralıkla en az bir an paylaşılıyor mu (yarı açık; uç uca değmek örtüşme değildir).</summary>
    public bool Overlaps(Interval interval)
    {
        if (interval.Start >= interval.End)
        {
            return false;
        }

        var index = LastStartingAtOrBefore(interval.Start);
        if (index >= 0 && _items[index].End > interval.Start)
        {
            return true;
        }

        var next = index + 1;
        return next < _items.Length && _items[next].Start < interval.End;
    }

    public bool Equals(IntervalSet? other) => other is not null && _items.AsSpan().SequenceEqual(other._items);

    public override bool Equals(object? obj) => Equals(obj as IntervalSet);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var item in _items)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }

    public override string ToString() => IsEmpty ? "{}" : string.Join(" ∪ ", _items.Select(i => $"[{i.Start}, {i.End})"));

    private static Instant Max(Instant a, Instant b) => a > b ? a : b;

    private static Instant Min(Instant a, Instant b) => a < b ? a : b;

    /// <summary>Başlangıcı <paramref name="instant"/> veya öncesinde olan son aralığın sırası; yoksa -1.</summary>
    private int LastStartingAtOrBefore(Instant instant)
    {
        int low = 0, high = _items.Length - 1, found = -1;
        while (low <= high)
        {
            var mid = low + ((high - low) / 2);
            if (_items[mid].Start <= instant)
            {
                found = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return found;
    }
}
