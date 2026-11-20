using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Markanın müşterisi (alan-modeli.md § BrandCustomer): salon panelindeki "Müşteri". Not, tercih ve sayaçlar buradadır;
/// başka marka görmez. (BrandId, CustomerId) tektir.
/// </summary>
internal sealed class BrandCustomer
{
    private BrandCustomer()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CustomerId { get; private set; }

    /// <summary>Salonun kendi yazdığı ad; müşterinin kendi verdiği addan farklı olabilir.</summary>
    public string? DisplayName { get; private set; }

    /// <summary>
    /// Salonun girdiği doğum yılı (MusteriEkle). Global kişinin doğum yılı markaya gösterilmez (alan-modeli.md § Customer:
    /// markalar kaydın tamamını görmez), bu yüzden salonun girdiği değer kartta tutulur (T-46).
    /// </summary>
    public int? BirthYear { get; private set; }

    /// <summary>Serbest metin; sağlık bilgisi yazılmaz (arayüz uyarır). Günlüğe yazılmaz.</summary>
    public string? Notes { get; private set; }

    public BrandCustomerSource Source { get; private set; }

    public Guid? PreferredStaffId { get; private set; }

    public LocalDate? FirstVisitOn { get; private set; }

    public LocalDate? LastVisitOn { get; private set; }

    public int VisitCount { get; private set; }

    public int CancelCount { get; private set; }

    public int NoShowCount { get; private set; }

    public long TotalSpentMinor { get; private set; }

    public bool IsBlocked { get; private set; }

    public string? BlockReason { get; private set; }

    /// <summary>
    /// "Randevu bilgilendirme SMS'leri gönderilsin" (MusteriEkle; sözleşme CustomerCreate.sendAppointmentSms).
    /// Alan modelinde yoktu; panelden eklenen müşterinin tercihi kartta saklanır (T-46). Rıza değildir: randevu
    /// bildirimleri işlem mesajıdır; bu tercih yalnız panelden oluşturulan randevularda varsayılan seçimi belirler.
    /// </summary>
    public bool AppointmentSmsEnabled { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public uint Version { get; private set; }

    public static BrandCustomer Create(
        Guid id,
        Guid brandId,
        Guid customerId,
        string? displayName,
        int? birthYear,
        string? notes,
        BrandCustomerSource source,
        bool appointmentSmsEnabled,
        Instant now) => new()
        {
            Id = id,
            BrandId = brandId,
            CustomerId = customerId,
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim(),
            BirthYear = birthYear,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Source = source,
            AppointmentSmsEnabled = appointmentSmsEnabled,
            CreatedUtc = now,
        };

    /// <summary>
    /// Sayaç farkını uygular (olaylarla güncellenen türetilmiş sayaçlar). Sayaçlar sıfırın altına inmez. Ziyaret günü
    /// yalnız ziyaret eklenirken ilk/son ziyaret gününü genişletir; geri almada günler yeniden hesaplanmaz (randevu
    /// geçmişi Scheduling'dedir).
    /// </summary>
    public void ApplyCounters(int visits, int cancels, int noShows, long spentMinor, LocalDate? visitedOn)
    {
        VisitCount = Math.Max(0, VisitCount + visits);
        CancelCount = Math.Max(0, CancelCount + cancels);
        NoShowCount = Math.Max(0, NoShowCount + noShows);
        TotalSpentMinor = Math.Max(0, TotalSpentMinor + spentMinor);
        if (visits > 0 && visitedOn is { } day)
        {
            FirstVisitOn = FirstVisitOn is { } first && first <= day ? first : day;
            LastVisitOn = LastVisitOn is { } last && last >= day ? last : day;
        }
    }
}
