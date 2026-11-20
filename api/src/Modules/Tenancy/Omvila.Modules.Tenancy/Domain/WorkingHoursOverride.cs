using System.Globalization;
using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Şubenin özel günü (alan-modeli.md § WorkingHoursOverride; tarih başına bir satır). Açık günün aralıkları
/// "HH:mm-HH:mm" metin dizisi olarak saklanır (sözleşmede gün başına birden çok aralık olabilir).
/// Personel özel günü (StaffBranchId) personel uçlarıyla gelir.
/// </summary>
internal sealed class WorkingHoursOverride
{
    private static readonly LocalTimePattern Time = LocalTimePattern.CreateWithInvariantCulture("HH:mm");

    private WorkingHoursOverride()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public LocalDate Date { get; private set; }

    public OverrideKind Kind { get; private set; }

    public bool IsClosed { get; private set; }

    public string[] Intervals { get; private set; } = [];

    public string? Note { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public IEnumerable<(LocalTime Start, LocalTime End)> OpeningIntervals => Intervals.Select(text =>
    {
        var parts = text.Split('-');
        return (Time.Parse(parts[0]).Value, Time.Parse(parts[1]).Value);
    });

    public static WorkingHoursOverride ForBranch(Guid id, Guid brandId, Guid branchId, DayOverride day, Instant now)
    {
        ArgumentNullException.ThrowIfNull(day);
        return new WorkingHoursOverride
        {
            Id = id,
            BrandId = brandId,
            BranchId = branchId,
            Date = day.Date,
            Kind = day.Kind,
            IsClosed = day.IsClosed,
            Intervals = [.. day.Intervals.Select(i => string.Create(CultureInfo.InvariantCulture, $"{Time.Format(i.Start)}-{Time.Format(i.End)}"))],
            Note = day.Note,
            CreatedUtc = now,
        };
    }
}
