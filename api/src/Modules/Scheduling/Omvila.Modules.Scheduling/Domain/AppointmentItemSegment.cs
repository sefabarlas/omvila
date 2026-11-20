using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kalemin meşguliyet dilimi (alan-modeli.md § AppointmentItemSegment). Çakışma kısıtı yalnız aktif ve personel-meşgul
/// dilimleri kapsar (musaitlik.md § Katman 1). Saat değişikliğinde eski dilimler pasifleşir ve <see cref="SupersededUtc"/>
/// alır; böylece "Geldiyse geri al" (A9) yalnız güncel dilimleri yeniden etkinleştirir.
/// </summary>
internal sealed class AppointmentItemSegment
{
    private AppointmentItemSegment()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid AppointmentItemId { get; private set; }

    public int SeqNo { get; private set; }

    public SegmentKind Kind { get; private set; }

    /// <summary>Kalemden kopya; kısıt şubeyle filtrelenmez (aynı personel aynı anda iki şubede olamaz).</summary>
    public Guid? StaffId { get; private set; }

    /// <summary>[Start, End) yarı açık; PostgreSQL tstzrange.</summary>
    public Interval During { get; private set; }

    public bool StaffBusy { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>Saat değişikliğiyle yerini yeni dilime bıraktığı an; dolu dilim bir daha etkinleşmez (T-47).</summary>
    public Instant? SupersededUtc { get; private set; }

    internal static AppointmentItemSegment Create(Guid id, Guid brandId, Guid itemId, Guid? staffId, PlannedSegment planned) => new()
    {
        Id = id,
        BrandId = brandId,
        AppointmentItemId = itemId,
        SeqNo = planned.SeqNo,
        Kind = planned.Kind,
        StaffId = staffId,
        During = planned.During,
        StaffBusy = planned.StaffBusy,
        IsActive = true,
    };

    internal void Deactivate() => IsActive = false;

    internal void Reactivate()
    {
        if (SupersededUtc is null)
        {
            IsActive = true;
        }
    }

    internal void Supersede(Instant now)
    {
        IsActive = false;
        SupersededUtc = now;
    }
}
