using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Tekrarlayan randevu (alan-modeli.md § AppointmentSeries). İlk 3 ay somut randevu satırı olarak üretilir; günlük iş
/// ufku ileri kaydırır (<see cref="GeneratedUntil"/>). "Yalnızca bu" değişikliği seriyi değiştirmez; "bu ve sonrakiler"
/// seriyi o tarihten böler. Geçmiş randevular etkilenmez.
/// </summary>
internal sealed class AppointmentSeries
{
    private readonly List<SeriesItemTemplate> _templateItems = [];

    private AppointmentSeries()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid BrandCustomerId { get; private set; }

    public IReadOnlyList<SeriesItemTemplate> TemplateItems => _templateItems;

    /// <summary>RFC 5545 RRULE (<see cref="WeeklyRecurrence"/>).</summary>
    public string RecurrenceRule { get; private set; } = string.Empty;

    public LocalDate StartsOn { get; private set; }

    public LocalTime LocalStartTime { get; private set; }

    public LocalDate? EndsOn { get; private set; }

    public SeriesStatus Status { get; private set; }

    /// <summary>Somut randevuların üretildiği son yerel tarih (dahil).</summary>
    public LocalDate GeneratedUntil { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant? UpdatedUtc { get; private set; }

    public uint Version { get; private set; }

    public WeeklyRecurrence Rule => WeeklyRecurrence.Parse(RecurrenceRule);

    public static AppointmentSeries Create(
        Guid id,
        Guid brandId,
        Guid branchId,
        Guid customerId,
        Guid brandCustomerId,
        IReadOnlyList<SeriesItemTemplate> templateItems,
        WeeklyRecurrence rule,
        LocalDate startsOn,
        LocalTime localStartTime,
        LocalDate? endsOn,
        LocalDate generatedUntil,
        Instant now)
    {
        ArgumentNullException.ThrowIfNull(templateItems);
        if (templateItems.Count == 0)
        {
            throw new DomainException("Serinin en az bir şablon kalemi olmalı.");
        }

        if (endsOn is { } end && end < startsOn)
        {
            throw new DomainException("Seri başlangıcından önce bitemez.");
        }

        var series = new AppointmentSeries
        {
            Id = id,
            BrandId = brandId,
            BranchId = branchId,
            CustomerId = customerId,
            BrandCustomerId = brandCustomerId,
            RecurrenceRule = rule.ToRRule(),
            StartsOn = startsOn,
            LocalStartTime = localStartTime,
            EndsOn = endsOn,
            Status = SeriesStatus.Active,
            GeneratedUntil = generatedUntil,
            CreatedUtc = now,
        };
        series._templateItems.AddRange(templateItems);
        return series;
    }

    /// <summary>Ufuk ileri kaydırıldı (ExtendRecurringSeries).</summary>
    public void MarkGeneratedUntil(LocalDate until, Instant now)
    {
        if (until > GeneratedUntil)
        {
            GeneratedUntil = until;
            UpdatedUtc = now;
        }
    }

    /// <summary>KVKK silmesi: seri biter, yeni randevu üretilmez (durum-makinesi.md § 10: → Ended).</summary>
    public void End(Instant now)
    {
        if (Status == SeriesStatus.Ended)
        {
            return;
        }

        Status = SeriesStatus.Ended;
        UpdatedUtc = now;
    }

    /// <summary>
    /// "Bu ve sonraki randevular" iptali: seri <paramref name="date"/>'ten önceki günde biter. İlk tarihten itibaren
    /// kesilirse seri tümüyle biter (<see cref="SeriesStatus.Ended"/>).
    /// </summary>
    public void EndBefore(LocalDate date, Instant now)
    {
        var last = date.PlusDays(-1);
        if (EndsOn is null || last < EndsOn)
        {
            EndsOn = last;
        }

        if (GeneratedUntil > last)
        {
            GeneratedUntil = last;
        }

        if (date <= StartsOn)
        {
            Status = SeriesStatus.Ended;
        }

        UpdatedUtc = now;
    }

    /// <summary>
    /// "Bu ve sonraki randevular" saat değişikliği: bu seri <paramref name="date"/>'ten önce biter, aynı kuralla
    /// <paramref name="date"/>'ten başlayan yeni seri döner. Yeni serinin ufku bu serinin ufkunu devralır.
    /// </summary>
    public AppointmentSeries SplitFrom(
        LocalDate date, Guid newId, LocalTime newLocalStartTime, IReadOnlyList<SeriesItemTemplate> templateItems, Instant now)
    {
        if (date < StartsOn || (EndsOn is { } end && date > end))
        {
            throw new DomainException("Bölme tarihi serinin içinde olmalı.");
        }

        var generatedUntil = GeneratedUntil < date ? date : GeneratedUntil;
        var next = Create(newId, BrandId, BranchId, CustomerId, BrandCustomerId, templateItems, Rule, date, newLocalStartTime, EndsOn, generatedUntil, now);
        EndBefore(date, now);
        return next;
    }
}
