using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>İzin (alan-modeli.md § TimeOff). Bütün şubelerde geçerlidir; randevuları iptal etmez.</summary>
internal sealed class TimeOff
{
    private TimeOff()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid StaffId { get; private set; }

    public TimeOffType Type { get; private set; }

    public Instant StartUtc { get; private set; }

    public Instant EndUtc { get; private set; }

    public bool IsAllDay { get; private set; }

    public string? Note { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static TimeOff Create(Guid id, Guid brandId, Guid staffId, TimeOffType type, TimeOffPeriod period, string? note, Instant now)
    {
        ArgumentNullException.ThrowIfNull(period);
        return new TimeOff
        {
            Id = id,
            BrandId = brandId,
            StaffId = staffId,
            Type = type,
            StartUtc = period.StartUtc,
            EndUtc = period.EndUtc,
            IsAllDay = period.IsAllDay,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            CreatedUtc = now,
        };
    }
}
