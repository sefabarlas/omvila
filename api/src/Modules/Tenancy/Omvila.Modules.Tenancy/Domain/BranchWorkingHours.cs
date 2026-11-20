using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Şubenin haftalık saat satırı (alan-modeli.md § BranchWorkingHours): gün başına bir veya daha çok aralık.</summary>
internal sealed class BranchWorkingHours
{
    private BranchWorkingHours()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public IsoDayOfWeek DayOfWeek { get; private set; }

    public LocalTime StartTime { get; private set; }

    public LocalTime EndTime { get; private set; }

    public static IEnumerable<BranchWorkingHours> From(Guid brandId, Guid branchId, WeeklyHours hours, Func<Guid> newId)
    {
        ArgumentNullException.ThrowIfNull(hours);
        ArgumentNullException.ThrowIfNull(newId);
        return hours.Days.SelectMany(day => day.Value.Select(interval => new BranchWorkingHours
        {
            Id = newId(),
            BrandId = brandId,
            BranchId = branchId,
            DayOfWeek = day.Key,
            StartTime = interval.Start,
            EndTime = interval.End,
        }));
    }
}
