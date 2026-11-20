using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Personelin bir şubedeki haftalık saat satırı (alan-modeli.md § StaffWorkingHours).</summary>
internal sealed class StaffWorkingHours
{
    private StaffWorkingHours()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid StaffBranchId { get; private set; }

    public IsoDayOfWeek DayOfWeek { get; private set; }

    public LocalTime StartTime { get; private set; }

    public LocalTime EndTime { get; private set; }

    public static StaffWorkingHours Create(Guid id, Guid brandId, Guid staffBranchId, IsoDayOfWeek day, LocalTime start, LocalTime end) => new()
    {
        Id = id,
        BrandId = brandId,
        StaffBranchId = staffBranchId,
        DayOfWeek = day,
        StartTime = start,
        EndTime = end,
    };
}
