using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Randevu kalemi (alan-modeli.md § AppointmentItem): kendi personeli, saati ve fiyatı vardır. Fiyat ve hizmet adı
/// oluşturma anında kopyalanır; hizmet sonradan değişse de kalem değişmez (dilim-02 kabul kriteri 2).
/// </summary>
internal sealed class AppointmentItem
{
    private readonly List<AppointmentItemSegment> _segments = [];

    private AppointmentItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid AppointmentId { get; private set; }

    public int SeqNo { get; private set; }

    public Guid ServiceId { get; private set; }

    /// <summary><c>null</c> yalnız personelsiz hizmette.</summary>
    public Guid? StaffId { get; private set; }

    public Instant StartUtc { get; private set; }

    public Instant EndUtc { get; private set; }

    /// <summary>Kopyalanan kalem fiyatı: şube fiyatı ?? hizmet fiyatı, personel ek ücreti dahil (T-47).</summary>
    public long PriceMinor { get; private set; }

    /// <summary><see cref="PriceMinor"/> içindeki personel ek ücreti (kopya).</summary>
    public long StaffSurchargeMinor { get; private set; }

    public long DiscountMinor { get; private set; }

    /// <summary>Paketten düşülüyorsa müşterinin paket kalemi; tahsil edilecek tutarı 0'dır ("Paketten").</summary>
    public Guid? CustomerPackageItemId { get; private set; }

    /// <summary>Kalem paketten düşülüyor mu.</summary>
    public bool IsFromPackage => CustomerPackageItemId is not null;

    public AppointmentItemStatus Status { get; private set; }

    public string ServiceNameSnapshot { get; private set; } = string.Empty;

    public IReadOnlyList<AppointmentItemSegment> Segments => _segments;

    /// <summary>Çakışma açısından geçerli (yerine yenisi yazılmamış) dilimler.</summary>
    public IEnumerable<AppointmentItemSegment> CurrentSegments => _segments.Where(s => s.SupersededUtc is null).OrderBy(s => s.SeqNo);

    /// <summary>Tahsil edilecek kalem tutarı (fiyat − indirim).</summary>
    public long NetAmountMinor => PriceMinor - DiscountMinor;

    internal static AppointmentItem Create(Guid id, Guid brandId, Guid appointmentId, ItemPlan plan, Func<Guid> newId)
    {
        var item = new AppointmentItem
        {
            Id = id,
            BrandId = brandId,
            AppointmentId = appointmentId,
            SeqNo = plan.SeqNo,
            ServiceId = plan.Offer.ServiceId,
            PriceMinor = plan.Offer.Price.AmountMinor,
            StaffSurchargeMinor = plan.Offer.StaffSurcharge.AmountMinor,
            Status = AppointmentItemStatus.Active,
            ServiceNameSnapshot = plan.Offer.ServiceName.Trim(),
        };
        item.Place(plan, newId);
        return item;
    }

    /// <summary>
    /// A14: eski dilimler pasif, yenileri aktif (aynı transaction). Fiyat kopyası korunur; personel değişse de ek ücret
    /// yeniden hesaplanmaz (T-47).
    /// </summary>
    internal void Reschedule(ItemPlan plan, Func<Guid> newId, Instant now)
    {
        foreach (var segment in _segments.Where(s => s.SupersededUtc is null))
        {
            segment.Supersede(now);
        }

        Place(plan, newId);
    }

    internal void Complete() => Status = AppointmentItemStatus.Completed;

    /// <summary>Kalemi paketten düşer (seans Sales'te ayrılır). Fiyat kopyası korunur, tahsil edilecek tutar 0 olur.</summary>
    // Kural: alan-modeli.md § AppointmentItem ("Paketten düşülen kalemde PriceMinor yine yazılır, tahsil edilecek tutar 0 olur")
    internal void UsePackage(Guid customerPackageItemId)
    {
        if (customerPackageItemId == Guid.Empty || CustomerPackageItemId is not null)
        {
            throw new DomainException("Kalem zaten paketten düşülüyor veya paket kalemi boş.");
        }

        CustomerPackageItemId = customerPackageItemId;
    }

    internal void Cancel()
    {
        Status = AppointmentItemStatus.Cancelled;
        DeactivateSegments();
    }

    internal void DeactivateSegments()
    {
        foreach (var segment in _segments)
        {
            segment.Deactivate();
        }
    }

    internal void ReactivateSegments()
    {
        foreach (var segment in _segments)
        {
            segment.Reactivate();
        }
    }

    private void Place(ItemPlan plan, Func<Guid> newId)
    {
        StaffId = plan.Offer.StaffId;
        StartUtc = plan.StartUtc;
        EndUtc = plan.EndUtc;
        foreach (var segment in plan.Segments)
        {
            _segments.Add(AppointmentItemSegment.Create(newId(), BrandId, Id, StaffId, segment));
        }
    }
}
