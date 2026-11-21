using NodaTime;
using Omvila.SharedKernel;

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

    /// <summary>"Kayıtları birleştir" ile arşivlendiyse kalan kart (alan-modeli.md § Birleştirme). Arşiv kartı listede görünmez.</summary>
    public Guid? MergedIntoId { get; private set; }

    public Instant? MergedUtc { get; private set; }

    /// <summary>KVKK silmesiyle anonimleştirildiyse (guvenlik-kvkk.md § Anonimleştirme).</summary>
    public Instant? AnonymizedUtc { get; private set; }

    public uint Version { get; private set; }

    /// <summary>Arşivlenmiş veya anonimleştirilmiş kart birleştirmeye ve silmeye kapalıdır.</summary>
    public bool IsActive => MergedIntoId is null && AnonymizedUtc is null;

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
    /// "Kayıtları birleştir": bu kart (kalacak kayıt) <paramref name="source"/>'u (birleşecek kayıt) devralır. Sayaçlar
    /// toplanır, ilk/son ziyaret genişler; boş doğum yılı ve tercih edilen personel kaynaktan dolar. <paramref name="mergeNotes"/>
    /// ise iki kartın notları kart oluşturma sırasıyla (eskisi önce) birleşir; sınırı aşarsa <c>mergeNotes: too_long</c>.
    /// Kaynak arşivlenir (<see cref="MergedIntoId"/>), silinmez. Arşivlenmiş/anonimleştirilmiş kaynak veya hedef
    /// <c>invalid_state_transition</c>.
    /// </summary>
    // Kural: alan-modeli.md § Birleştirme
    public Result Absorb(BrandCustomer source, bool mergeNotes, Instant now)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Id == Id || source.BrandId != BrandId)
        {
            return Error.Validation("sourceBrandCustomerId", "invalid_format");
        }

        if (!IsActive || !source.IsActive)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        string? notes = Notes;
        if (mergeNotes && source.Notes is not null)
        {
            var parts = CreatedUtc <= source.CreatedUtc ? new[] { Notes, source.Notes } : [source.Notes, Notes];
            notes = string.Join("\n\n", parts.Where(p => p is not null));
            if (notes.Length > CustomerRules.NotesMaxLength)
            {
                return Error.Validation("mergeNotes", "too_long");
            }
        }

        Notes = notes;
        VisitCount += source.VisitCount;
        CancelCount += source.CancelCount;
        NoShowCount += source.NoShowCount;
        TotalSpentMinor += source.TotalSpentMinor;
        FirstVisitOn = Min(FirstVisitOn, source.FirstVisitOn);
        LastVisitOn = Max(LastVisitOn, source.LastVisitOn);
        BirthYear ??= source.BirthYear;
        PreferredStaffId ??= source.PreferredStaffId;
        source.MergedIntoId = Id;
        source.MergedUtc = now;
        return Result.Success();
    }

    /// <summary>
    /// KVKK silmesi (salon tarafı, guvenlik-kvkk.md § Anonimleştirme): ad "Silinmiş müşteri", not, doğum yılı, engel nedeni
    /// ve tercih silinir; kart anonim global kişiye bağlanır (gerçek kişiyle bağ kopar). Sayaçlar istatistik olarak kalır.
    /// </summary>
    // Kural: guvenlik-kvkk.md § Anonimleştirme
    public Result Anonymize(Guid anonymousCustomerId, Instant now)
    {
        if (!IsActive)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        CustomerId = anonymousCustomerId;
        DisplayName = Customer.AnonymizedName;
        Notes = null;
        BirthYear = null;
        PreferredStaffId = null;
        IsBlocked = false;
        BlockReason = null;
        AppointmentSmsEnabled = false;
        AnonymizedUtc = now;
        return Result.Success();
    }

    /// <summary>İçe aktarmada numarası kayıtlı kart: mevcut değerler korunur, yalnız boş not ve doğum yılı dolar.</summary>
    public void FillBlanks(string? notes, int? birthYear)
    {
        Notes ??= string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        BirthYear ??= birthYear;
    }

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

    private static LocalDate? Min(LocalDate? a, LocalDate? b) => a is null ? b : b is null ? a : LocalDate.Min(a.Value, b.Value);

    private static LocalDate? Max(LocalDate? a, LocalDate? b) => a is null ? b : b is null ? a : LocalDate.Max(a.Value, b.Value);
}
