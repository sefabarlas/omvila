using System.Text.RegularExpressions;
using Omvila.Modules.Notifications.Contracts;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Sistem SMS şablonları (docs/standartlar/bildirimler.md § Olay kataloğu, metinler birebir). Değişken <c>{ad}</c>;
/// <c>{ad|bulunma}</c> değere Türkçe bulunma ekini ekler ("14:30'da", "Kuaför Elif'te"; bildirimler.md § ek uyumu).
/// Kullanıcı verisinden gelen değişkenler (salon, hizmet, personel) şablonda "kırpılabilir" olarak işaretlenir: metin
/// 155 birimi aşarsa sırayla sondan kısaltılır ("şablon kısaltılır veya değişken kırpılır"). Salonun şablon ezmesi ve
/// NotificationTemplate tablosu D3'te.
/// </summary>
internal static partial class SmsTemplates
{
    /// <summary>Aynı gün hatırlatması (ReminderOffsetsMinutes 120): "Bugün {saat}'da {salon}'te görüşmek üzere…".</summary>
    public const string SameDayVariant = "same_day";

    /// <summary>Kalemlerde personel yoksa onay metni personel parantezi olmadan.</summary>
    public const string WithoutStaffVariant = "without_staff";

    // Kırpılan değişken en az bu kadar karakter kalır (okunabilirlik).
    private const int MinTrimmedLength = 4;

    private const int SalonNameLength = 30;

    // Desen yalnız sabit şablon metnine uygulanır; zaman aşımı analizör kuralı (MA0009) için üst sınırdır.
    private const int RegexTimeoutMilliseconds = 1000;

    // Aşımda önce salon adı 30 karaktere (bildirimler.md § Şablon testi: salon adı 30 karakter), sonra personel ve hizmet,
    // en son salon adı en kısaya kırpılır.
    private static readonly Trim[] SalonOnly = [new("salon", SalonNameLength), new("salon", MinTrimmedLength)];

    private static readonly Trim[] SalonUpTo30 = [new("salon", SalonNameLength)];

    // Talep iptali metinleri uzun: 30 karakterlik salon adı ve ondan türeyen rezervasyon bağlantısıyla 155'i aşarlar. Aşımda
    // salon adını kırpmak yerine bu kısa metin kullanılır, o da aşarsa salon adı kırpılır (T-65). "Reddedildi" geçmez.
    private static readonly Template RequestCancelledShort = new(
        "{salon}, {tarih} {saat} talebiniz iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}", SalonOnly);

    // Kapora metni (V2): IBAN boşluksuz yazılır; aşımda salon adı 30 karaktere, sonra alıcı adı, en son salon adı kırpılır.
    // IBAN, tutar, süre ve açıklama kodu kırpılmaz (dilim-04 kabul kriteri 3).
    private static readonly Trim[] DepositTrims = [new("salon", SalonNameLength), new("alıcı", MinTrimmedLength), new("salon", MinTrimmedLength)];

    private static readonly Dictionary<string, Template> Templates = new(StringComparer.Ordinal)
    {
        [Key(NotificationEvents.OtpCode, null)] = new("OMVILA giriş kodunuz: {kod}. Bu kodu kimseyle paylaşmayın.", []),
        [Key(NotificationEvents.StaffInvite, null)] = new("{salon}: Ekibimize eklendiniz. Bu numarayla OMVILA'ya giriş yapın: {bağlantı}", SalonOnly),
        [Key(NotificationEvents.AppointmentConfirmed, null)] = new(
            "{salon}: Randevunuz onaylandı. {tarih} {saat}, {hizmet} ({personel}). Değişiklik ve iptal: {bağlantı}",
            [new("salon", SalonNameLength), new("personel", MinTrimmedLength), new("hizmet", MinTrimmedLength), new("salon", MinTrimmedLength)]),
        [Key(NotificationEvents.AppointmentConfirmed, WithoutStaffVariant)] = new(
            "{salon}: Randevunuz onaylandı. {tarih} {saat}, {hizmet}. Değişiklik ve iptal: {bağlantı}",
            [new("salon", SalonNameLength), new("hizmet", MinTrimmedLength), new("salon", MinTrimmedLength)]),
        [Key(NotificationEvents.AppointmentRequested, null)] = new(
            "{salon}: Talebiniz alındı. {tarih} {saat} için salon onayladığında size haber vereceğiz. {bağlantı}", SalonOnly),

        // Katalogdaki "Görüşmek üzere!" ses tonu kuralıyla (ünlem yok) "Görüşmek üzere." yazılır (T-65).
        [Key(NotificationEvents.AppointmentApproved, null)] = new(
            "{salon} talebinizi onayladı: {tarih} {saat}, {hizmet}. Görüşmek üzere. {bağlantı}",
            [new("salon", SalonNameLength), new("hizmet", MinTrimmedLength), new("salon", MinTrimmedLength)]),
        [Key(NotificationEvents.AppointmentDeclined, null)] = new(
            "{salon}, {tarih} {saat} talebinizi onaylayamadı, talep iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}",
            SalonUpTo30,
            RequestCancelledShort),
        [Key(NotificationEvents.AppointmentApprovalExpired, null)] = new(
            "{salon}, {tarih} {saat} talebinizi zamanında yanıtlayamadı, talep iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}",
            SalonUpTo30,
            RequestCancelledShort),
        [Key(NotificationEvents.AppointmentRescheduled, null)] = new(
            "{salon}: Randevunuz {yeni tarih} {saat} olarak güncellendi. Detay: {bağlantı}", SalonOnly),
        [Key(NotificationEvents.AppointmentCancelledBySalon, null)] = new(
            "{salon}, {tarih} {saat} randevunuzu iptal etti. {rezervasyon bağlantısı}", SalonOnly),
        [Key(NotificationEvents.DepositAwaiting, null)] = new(
            "{salon}: {tarih} {saat} için {tutar} TL kaporayı {süre} içinde gönderin. IBAN {iban}, {alıcı}, açıklama {kod}", DepositTrims),
        [Key(NotificationEvents.AppointmentReminder, null)] = new(
            "Hatırlatma: Yarın {saat|bulunma} {salon|bulunma} randevunuz var. Değişiklik için: {bağlantı}", SalonOnly),
        [Key(NotificationEvents.AppointmentReminder, SameDayVariant)] = new(
            "Bugün {saat|bulunma} {salon|bulunma} görüşmek üzere. Yol tarifi: {bağlantı}", SalonOnly),
    };

    /// <summary>Tanımlı (olay kodu, çeşit) çiftleri.</summary>
    public static IReadOnlyList<(string EventCode, string? Variant)> Keys =>
        [.. Templates.Keys.Select(k => k.Split('#') is [var code, var variant] ? (code, (string?)variant) : (k, null))];

    public static string Render(string eventCode, IReadOnlyDictionary<string, string> values, string? variant = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (!Templates.TryGetValue(Key(eventCode, variant), out var template))
        {
            throw new InvalidOperationException($"SMS şablonu yok: {eventCode} {variant}");
        }

        var body = Render(eventCode, template, values);
        return template.Fallback is { } fallback && GsmLength.Units(body) is { } units && units > GsmLength.SingleSmsLimit
            ? Render(eventCode, fallback, values)
            : body;
    }

    private static string Render(string eventCode, Template template, IReadOnlyDictionary<string, string> values)
    {
        var current = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Match match in Placeholder().Matches(template.Body))
        {
            var name = match.Groups["name"].Value;
            if (!values.TryGetValue(name, out var value))
            {
                throw new InvalidOperationException($"Şablonda doldurulmamış değişken: {eventCode} {name}");
            }

            current[name] = value;
        }

        var body = Fill(template.Body, current);
        foreach (var (name, min) in template.Trims)
        {
            while (GsmLength.Units(body) is { } units && units > GsmLength.SingleSmsLimit && current[name].Length > min)
            {
                var value = current[name];
                var keep = Math.Max(min, value.Length - (units - GsmLength.SingleSmsLimit));
                current[name] = value[..keep].TrimEnd(' ', ',', '.', '-', '+', '(', '&');
                body = Fill(template.Body, current);
            }
        }

        return body;
    }

    private static string Fill(string body, Dictionary<string, string> values) =>
        Placeholder().Replace(body, match =>
        {
            var value = values[match.Groups["name"].Value];
            return match.Groups["helper"].Value switch
            {
                "" => value,
                "bulunma" => TurkishSuffix.Locative(value),
                var helper => throw new InvalidOperationException($"Bilinmeyen şablon yardımcısı: {helper}"),
            };
        });

    private static string Key(string eventCode, string? variant) => variant is null ? eventCode : eventCode + "#" + variant;

    [GeneratedRegex(@"\{(?<name>[^{}|]+)(\|(?<helper>[^{}]+))?\}", RegexOptions.ExplicitCapture, RegexTimeoutMilliseconds)]
    private static partial Regex Placeholder();

    /// <param name="Body">Metin.</param>
    /// <param name="Trims">Aşımda sırayla uygulanan kırpmalar.</param>
    /// <param name="Fallback">Kırpmalardan sonra da aşarsa kullanılan kısa metin (aynı değişkenlerle).</param>
    private sealed record Template(string Body, Trim[] Trims, Template? Fallback = null);

    /// <param name="Name">Değişken.</param>
    /// <param name="MinLength">Bu adımda inilebilecek en kısa uzunluk.</param>
    private sealed record Trim(string Name, int MinLength);
}
