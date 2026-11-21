using NodaTime;
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
        new("Renk", "Balyaj", "Uzunluğa göre", 150, 250000, StartingFrom: true, RequiresApproval: true, ["Elif Kaya", "Zeynep Aksoy"]),
        new("Bakım", "Saç bakımı", "Keratin maske", 45, 60000, StartingFrom: false, RequiresApproval: null, ["Elif Kaya", "Zeynep Aksoy"]),
        new("Bakım", "Keratin", "Uzunluğa göre", 90, 180000, StartingFrom: true, RequiresApproval: true, ["Elif Kaya", "Zeynep Aksoy"]),
        new("Tırnak", "Manikür", null, 45, 40000, StartingFrom: false, RequiresApproval: null, ["Zeynep Aksoy", "Can Yücel"]),
        new("Tırnak", "Kaş ve kirpik", null, 30, 35000, StartingFrom: false, RequiresApproval: null, ["Zeynep Aksoy"]),
    ];

    /// <summary>Müşteriler ekranı ve takvimdeki adlar. İlk iki "2 kez gelmedi" olacak müşteridir (Musteriler.png).</summary>
    public static readonly IReadOnlyList<string> Customers =
    [
        "Gizem Arslan", "Ece Koç", "Selin Öztürk", "Derya Şahin", "Pınar Yalçın", "Buse Kaya", "Melis Tunç", "Nur Ateş",
        "Ayşe Yılmaz", "Hande Er", "Kerem Aydın", "Aslı Kurt", "Tuğba Er", "Kaan Polat", "Deniz Acar",
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

    internal sealed record ServiceSeed(
        string Category, string Name, string? Description, int DurationMinutes, long PriceMinor, bool StartingFrom, bool? RequiresApproval, IReadOnlyList<string> Staff);
}
