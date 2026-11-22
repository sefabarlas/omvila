using NodaTime;
using Omvila.Modules.Scheduling.DemoData;
using Omvila.Modules.Tenancy.DemoData;
using Omvila.SharedKernel;

namespace Omvila.Host.Seeding;

/// <summary>
/// Demo verisinin içeriği (acik-sorular.md T-74): tasarım ekranlarındaki salon, personel, hizmet ve müşteri adları
/// (Takvim, GenelBakis, Personel, Hizmetler, Musteriler). Bu sınıf örnek veridir, ürün kuralı değildir: fiyat ve süreler
/// tasarımdaki örneklerdir, gerçek kişi yoktur. Telefonlar ayrılmış demo aralığındadır: personel +90 555 000 00xx,
/// müşteriler +90 555 001 00xx.
/// </summary>
internal static class DemoDataSet
{
    public const string BusinessName = "Kuaför Elif";
    public const string IndustryCode = "hair_salon";

    /// <summary>Rezervasyon sayfası bağlantı adı (işletme adından türer; başka markada alınmışsa seed durur).</summary>
    public const string Slug = "kuafor-elif";

    public const string OwnerName = "Elif Kaya";
    public const string OwnerTitle = "Kurucu · kuaför";

    /// <summary>Onay yanıt süresi: izinli en uzun değer (BrandSettingsRules: 30, 60, 120, 240); talepler demoda daha uzun bekler.</summary>
    public const int ApprovalTimeoutMinutes = 240;

    /// <summary>Hedef SMS kontör bakiyesi (LowCreditThreshold üstünde: "Kontör az" şeridi çıkmaz).</summary>
    public const int SmsCredits = 1000;

    /// <summary>Randevu penceresi: bugünden önceki ve sonraki gün sayısı.</summary>
    public const int DaysBack = 14;
    public const int DaysAhead = 14;

    /// <summary>Onay bekleyen talepler personelin yarından sonraki ilk bu kadar çalışma gününe yazılır (Elif ve Zeynep: 4 talep).</summary>
    public const int PendingDays = 2;

    public static readonly PhoneNumber OwnerPhone = PhoneNumber.Parse("+905550000001");

    /// <summary>
    /// Kapora hesabı (AyarlarOdeme). IBAN sözleşme örneğindeki uydurma, mod-97 geçerli test IBAN'ıdır; banka adı tasarımdaki
    /// örnek. Havale süresi varsayılan 120 dk: kapora bekleyen demo talepleri seed'den 2 saat sonra otomatik iptal olur (A5).
    /// </summary>
    public static readonly DemoBankAccount BankAccount = new(OwnerName, "Ziraat Bankası", "TR290006400000112345678901", "OMV", 120);

    /// <summary>Kapora politikası (sureler-ve-ayarlar.md § Kapora varsayılanı) ve riskli müşteri eşiği: 2 kez gelmeyen.</summary>
    public const string DepositPolicy = "RiskyCustomersOnly";
    public const int RiskyCustomerNoShowThreshold = 2;

    /// <summary>
    /// Kaporalı demo randevuları, sırasıyla: online randevu alan personelin yarından sonraki ilk <see cref="PendingDays"/> çalışma
    /// gününde günün ikinci randevusu, kaporalı hizmetle ve riskli (2 kez gelmemiş) müşteriyle.
    /// </summary>
    public static readonly IReadOnlyList<DemoAppointmentOutcome> DepositOutcomes =
    [
        DemoAppointmentOutcome.DepositAwaiting, DemoAppointmentOutcome.DepositReported,
        DemoAppointmentOutcome.DepositReceived, DemoAppointmentOutcome.DepositWaived,
    ];

    public static readonly DemoBranch Kadikoy = new(
        "Kadıköy", "Caferağa Mah. Moda Cad. No: 12", "Kadıköy", "İstanbul", Week(IsoDayOfWeek.Monday, IsoDayOfWeek.Saturday, 9, 20));

    public static readonly DemoBranch Nisantasi = new(
        "Nişantaşı", "Teşvikiye Mah. Abdi İpekçi Cad. No: 8", "Şişli", "İstanbul", Week(IsoDayOfWeek.Monday, IsoDayOfWeek.Saturday, 10, 19));

    /// <summary>Personel ekranındaki Elif Kaya saatleri (Pazartesi ve Pazar kapalı, öğle molası).</summary>
    public static readonly IReadOnlyList<DemoHours> OwnerHours =
    [
        .. new[] { IsoDayOfWeek.Tuesday, IsoDayOfWeek.Wednesday, IsoDayOfWeek.Thursday }.SelectMany(d => new[]
        {
            new DemoHours(d, new LocalTime(10, 0), new LocalTime(13, 0)),
            new DemoHours(d, new LocalTime(13, 45), new LocalTime(19, 0)),
        }),
        new DemoHours(IsoDayOfWeek.Friday, new LocalTime(10, 0), new LocalTime(14, 0)),
        new DemoHours(IsoDayOfWeek.Friday, new LocalTime(14, 45), new LocalTime(20, 0)),
        new DemoHours(IsoDayOfWeek.Saturday, new LocalTime(9, 0), new LocalTime(18, 0)),
    ];

    /// <summary>Sahip dışındaki personel. Takvim rengi personel verisidir (sözleşme calendarColor), tasarım tokenı değildir.</summary>
    public static readonly IReadOnlyList<StaffSeed> Staff =
    [
        new("Mert Demir", "Berber", "Staff", "+905550000002", "#B4532A", IsBookableOnline: true, Branch.Kadikoy, Week(IsoDayOfWeek.Monday, IsoDayOfWeek.Saturday, 9, 18)),
        new("Can Yücel", "Kuaför", "Staff", "+905550000003", "#2F5D8A", IsBookableOnline: false, Branch.Kadikoy, Week(IsoDayOfWeek.Monday, IsoDayOfWeek.Saturday, 11, 20)),
        new("Zeynep Aksoy", "Renk ve bakım", "BranchManager", "+905550000004", "#8A6D3B", IsBookableOnline: true, Branch.Nisantasi, Week(IsoDayOfWeek.Monday, IsoDayOfWeek.Saturday, 10, 19)),
    ];

    public static readonly IReadOnlyList<string> Categories = ["Saç", "Renk", "Bakım", "Tırnak"];

    /// <summary>Hizmetler; personel adlarıyla eşlenir. Onay gerektiren hizmetler (Keratin, Balyaj) online talepte Onay bekliyor olur.</summary>
    public static readonly IReadOnlyList<ServiceSeed> Services =
    [
        new("Saç", "Saç kesimi", null, 45, 45000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Mert Demir", "Can Yücel"]),
        new("Saç", "Fön", "Kısa ve orta saç", 30, 25000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Zeynep Aksoy", "Mert Demir"]),
        new("Saç", "Sakal ve kesim", null, 60, 55000, StartingFrom: false, RequiresApproval: null, ["Mert Demir"]),
        new("Renk", "Boya", null, 90, 120000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Zeynep Aksoy", "Can Yücel"]),
        new("Renk", "Dip boya", null, 60, 90000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Zeynep Aksoy"]),
        new("Renk", "Balyaj", "Uzunluğa göre", 150, 250000, StartingFrom: true, RequiresApproval: true, ["Elif Kaya", "Zeynep Aksoy"], DepositPercent: 30),
        new("Bakım", "Saç bakımı", "Keratin maske", 45, 60000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Zeynep Aksoy"]),
        new("Bakım", "Keratin", "Uzunluğa göre", 90, 180000, StartingFrom: true, RequiresApproval: true, ["Elif Kaya", "Zeynep Aksoy"], DepositAmountMinor: 15000),
        new("Tırnak", "Manikür", null, 45, 40000, StartingFrom: false, RequiresApproval: null, ["Zeynep Aksoy", "Can Yücel"]),
        new("Tırnak", "Kaş ve kirpik", null, 30, 35000, StartingFrom: false, RequiresApproval: null, ["Zeynep Aksoy"]),
    ];

    /// <summary>Müşteriler ekranı ve takvimdeki adlar. İlk iki "2 kez gelmedi" olacak müşteridir (Musteriler.png).</summary>
    public static readonly IReadOnlyList<string> Customers =
    [
        "Gizem Arslan", "Ece Koç", "Selin Öztürk", "Derya Şahin", "Pınar Yalçın", "Buse Kaya", "Melis Tunç", "Nur Ateş",
        "Ayşe Yılmaz", "Hande Er", "Kerem Aydın", "Aslı Kurt", "Tuğba Er", "Kaan Polat", "Deniz Acar",
    ];

    /// <summary>
    /// Prim oranları (Personel düzenle; hizmet / satış yüzdesi). Elif Kaya %24 hizmet primi (dilim-05 KK5); satış primi pakete özel
    /// oran yoksa bu orandır.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (decimal Service, decimal Sale)> Commissions = new Dictionary<string, (decimal, decimal)>(StringComparer.Ordinal)
    {
        ["Elif Kaya"] = (24m, 5m),
        ["Mert Demir"] = (20m, 5m),
        ["Can Yücel"] = (20m, 5m),
        ["Zeynep Aksoy"] = (22m, 5m),
    };

    /// <summary>Paket tanımları (Paketler.png): 10 seanslık Fön paketi satış primi %10 (KK6); bakım paketi personelin oranıyla.</summary>
    public static readonly IReadOnlyList<PackageSeed> Packages =
    [
        new("Fön paketi", "Fön", 10, 225000, 180, SaleCommissionPercent: 10m),
        new("Saç bakımı paketi", "Saç bakımı", 4, 216000, 90, SaleCommissionPercent: null),
    ];

    /// <summary>
    /// Satılmış müşteri paketleri. <see cref="CustomerPackageSeed.Redeem"/> kadar seans geçmiş randevularda paketten düşülür (o
    /// hizmetin tamamlanan randevuları bu müşteriye yazılır): Selin Öztürk'te 2 seans kalır ("Az kaldı"), Derya Şahin'in paketi 10
    /// gün içinde biter ("Süresi yaklaşan"), Melis Tunç'a dün Elif Kaya sattı (Kasa'da paket satışı, %10 satış primi).
    /// </summary>
    public static readonly IReadOnlyList<CustomerPackageSeed> CustomerPackages =
    [
        new(Customer: 2, "Saç bakımı paketi", SoldDaysAgo: 20, "Elif Kaya", "Card", Redeem: 2),
        new(Customer: 3, "Fön paketi", SoldDaysAgo: 170, "Zeynep Aksoy", "Cash", Redeem: 1),
        new(Customer: 6, "Fön paketi", SoldDaysAgo: 1, "Elif Kaya", "Card", Redeem: 0),
    ];

    /// <summary>Tamamlanan randevuların tahsilat yöntemi sırayla (Kasa: nakit, kart, havale).</summary>
    public static readonly IReadOnlyList<string> PaymentMethods = ["Cash", "Card", "Cash", "Transfer"];

    /// <summary>Kapatılmış kasa günü (KK7 örneği): Kadıköy'de bugünden önceki son nakit tahsilatlı gün, sayılan beklenenden 50 ₺ az.</summary>
    public const long CashDayDifferenceMinor = -5000;

    public const string CashDayNote = "Para üstü eksik verildi";

    /// <summary>Gün kapatma saati (şube yerel; Kadıköy 20:00'de kapanır).</summary>
    public static readonly LocalTime CashDayClosedAt = new(20, 30);

    /// <summary>
    /// Değerlendirmeler, en yeni tamamlanan randevulardan başlayarak (Degerlendirmeler.png). İlki yanıtlı, düşük puanlı olan yanıt
    /// bekliyor.
    /// </summary>
    public static readonly IReadOnlyList<ReviewSeed> Reviews =
    [
        new(5, "Elif Hanım çok ilgiliydi, saçım tam istediğim gibi oldu.", "Teşekkür ederiz, yine bekleriz."),
        new(5, "Randevu saatinde başladı, hiç beklemedim.", null),
        new(4, "Sonuç güzel, yalnız biraz kalabalıktı.", null),
        new(2, "Fön beklediğimden kısa sürdü, kısa süre sonra dağıldı.", null),
        new(5, null, null),
        new(4, "Temiz ve düzenli bir salon.", null),
    ];

    /// <summary>Paket satış saati (şube yerel).</summary>
    public static readonly LocalTime PackageSoldAt = new(15, 0);

    /// <summary>Salonun yanıtının değerlendirmeden sonra yazıldığı süre.</summary>
    public static readonly Duration ReplyDelay = Duration.FromHours(1);

    /// <summary>Değerlendirmenin randevu bitişinden sonra yazıldığı süre.</summary>
    public static readonly Duration ReviewDelay = Duration.FromHours(2);

    /// <summary>Bekleme listesi (BeklemeListesi.png): Kadıköy'de iki kişi.</summary>
    public static readonly IReadOnlyList<WaitlistSeed> Waitlist =
    [
        new(Customer: 9, "Boya", "Elif Kaya", AcceptOtherStaff: false, DatesAhead: 3, ["Afternoon"], Online: true, JoinedAgo: Duration.FromDays(1)),
        new(Customer: 11, "Saç kesimi", null, AcceptOtherStaff: true, DatesAhead: 0, ["Evening"], Online: false, JoinedAgo: Duration.FromHours(3)),
    ];

    /// <summary>Kapıda sıra (KapidaSira.png): bugün Kadıköy'de iki bilet; ilki telefonsuz, adla.</summary>
    public static readonly IReadOnlyList<WalkInSeed> WalkIns =
    [
        new("Okan Er", Customer: null, "Saç kesimi", "Mert Demir", CreatedAgo: Duration.FromMinutes(25)),
        new(null, Customer: 13, "Sakal ve kesim", "Mert Demir", CreatedAgo: Duration.FromMinutes(10)),
    ];

    /// <summary>Demo müşterisinin telefonu: +90 555 001 00xx (sıra 1'den).</summary>
    public static PhoneNumber CustomerPhone(int index) => PhoneNumber.Parse($"+9055500100{index + 1:00}");

    private static List<DemoHours> Week(IsoDayOfWeek from, IsoDayOfWeek to, int startHour, int endHour) =>
        [.. Enumerable.Range((int)from, to - from + 1).Select(d => new DemoHours((IsoDayOfWeek)d, new LocalTime(startHour, 0), new LocalTime(endHour, 0)))];

    internal enum Branch
    {
        Kadikoy,
        Nisantasi,
    }

    internal sealed record StaffSeed(
        string FullName, string Title, string Role, string Phone, string CalendarColor, bool IsBookableOnline, Branch Branch, IReadOnlyList<DemoHours> Hours);

    internal sealed record PackageSeed(string Name, string Service, int SessionCount, long PriceMinor, int ValidityDays, decimal? SaleCommissionPercent);

    internal sealed record CustomerPackageSeed(int Customer, string Package, int SoldDaysAgo, string Seller, string Method, int Redeem);

    internal sealed record ReviewSeed(int Rating, string? Comment, string? Reply);

    internal sealed record WaitlistSeed(
        int Customer, string Service, string? Staff, bool AcceptOtherStaff, int DatesAhead, IReadOnlyList<string> TimeWindows, bool Online, Duration JoinedAgo);

    internal sealed record WalkInSeed(string? Name, int? Customer, string Service, string? Staff, Duration CreatedAgo);

    internal sealed record ServiceSeed(
        string Category, string Name, string? Description, int DurationMinutes, long PriceMinor, bool StartingFrom, bool? RequiresApproval, IReadOnlyList<string> Staff,
        decimal? DepositPercent = null, long? DepositAmountMinor = null)
    {
        public bool HasDeposit => DepositPercent is not null || DepositAmountMinor is not null;
    }
}
