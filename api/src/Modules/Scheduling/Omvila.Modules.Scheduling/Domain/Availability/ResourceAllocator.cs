using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Kaynak ihtiyacının karşılanması (musaitlik.md § 6): türün şubedeki kaynakları <c>Id</c> sırasıyla denenir; bakım dışı
/// aralığı ihtiyaç penceresine değen kaynak atlanır, diğerinden pencere boyunca boş kalan kapasite kadar birim alınır.
/// Aynı randevunun önceki kalemlerine ayrılan birimler de kullanım sayılır. Saf fonksiyon.
/// </summary>
internal static class ResourceAllocator
{
    public static bool TryAllocate(
        ResourceInventory inventory,
        IReadOnlyList<ResourceNeed> needs,
        ItemPlan plan,
        IReadOnlyList<ResourceAllocation> alreadyAllocated,
        Guid? ignoreAppointmentId,
        out IReadOnlyList<ResourceAllocation> allocations)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        ArgumentNullException.ThrowIfNull(needs);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(alreadyAllocated);
        if (needs.Count == 0)
        {
            allocations = [];
            return true;
        }

        var result = new List<ResourceAllocation>();
        foreach (var need in needs)
        {
            var window = NeedWindow(need, plan);
            var remaining = need.Quantity;
            foreach (var resource in inventory.Resources.Where(r => r.ResourceTypeId == need.ResourceTypeId).OrderBy(r => r.Id))
            {
                if (remaining <= 0)
                {
                    break;
                }

                if (inventory.Downtimes.Any(d => d.ResourceId == resource.Id && Overlaps(d.During, window)))
                {
                    continue;
                }

                var used = PeakUsage(resource.Id, window, inventory.Usages, alreadyAllocated.Concat(result), ignoreAppointmentId);
                var free = resource.EffectiveCapacity - used;
                if (free <= 0)
                {
                    continue;
                }

                var units = Math.Min(free, remaining);
                result.Add(new ResourceAllocation(resource.Id, units, window));
                remaining -= units;
            }

            if (remaining > 0)
            {
                allocations = [];
                return false;
            }
        }

        allocations = result;
        return true;
    }

    /// <summary>İhtiyacın zaman penceresi: aşama aralığı verilmişse o aşamalar, yoksa kalemin tamamı (tamponlar dahil).</summary>
    public static Interval NeedWindow(ResourceNeed need, ItemPlan plan)
    {
        ArgumentNullException.ThrowIfNull(need);
        ArgumentNullException.ThrowIfNull(plan);
        if (need.StageSeqFrom is null && need.StageSeqTo is null)
        {
            return new Interval(plan.StartUtc, plan.EndUtc);
        }

        var stages = plan.Segments.Where(s => s.Kind == SegmentKind.Stage).ToList();
        var from = Math.Clamp(need.StageSeqFrom ?? 1, 1, stages.Count);
        var to = Math.Clamp(need.StageSeqTo ?? stages.Count, from, stages.Count);
        return new Interval(stages[from - 1].During.Start, stages[to - 1].During.End);
    }

    private static int PeakUsage(
        Guid resourceId, Interval window, IEnumerable<ResourceBlock> usages, IEnumerable<ResourceAllocation> allocated, Guid? ignoreAppointmentId)
    {
        var events = new List<(Instant At, int Delta)>();
        foreach (var usage in usages)
        {
            if (usage.ResourceId == resourceId && Overlaps(usage.During, window) && (ignoreAppointmentId is null || usage.AppointmentId != ignoreAppointmentId))
            {
                Add(events, usage.During, window, 1);
            }
        }

        foreach (var allocation in allocated)
        {
            if (allocation.ResourceId == resourceId && Overlaps(allocation.During, window))
            {
                Add(events, allocation.During, window, allocation.Units);
            }
        }

        // Yarı açık aralık: aynı anda biten kullanım, başlayandan önce düşülür.
        events.Sort((a, b) => a.At != b.At ? a.At.CompareTo(b.At) : a.Delta.CompareTo(b.Delta));
        int current = 0, peak = 0;
        foreach (var (_, delta) in events)
        {
            current += delta;
            peak = Math.Max(peak, current);
        }

        return peak;
    }

    /// <summary>Kullanımı pencereye kırparak ekler; tepe yalnız pencere içinde ölçülür.</summary>
    private static void Add(List<(Instant At, int Delta)> events, Interval during, Interval window, int units)
    {
        events.Add((during.Start > window.Start ? during.Start : window.Start, units));
        events.Add((during.End < window.End ? during.End : window.End, -units));
    }

    private static bool Overlaps(Interval a, Interval b) => a.Start < b.End && b.Start < a.End;
}
