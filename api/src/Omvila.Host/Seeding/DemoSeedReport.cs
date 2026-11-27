using System.Globalization;
using NodaTime;
using Omvila.Modules.Scheduling.DemoData;
using Omvila.SharedKernel;

namespace Omvila.Host.Seeding;

/// <summary>
/// Seed sonucu: giriş telefonları, rezervasyon bağlantısı ve sayılar. Konsola yazılır (günlüğe değil): telefonlar ayrılmış
/// demo aralığındadır, gerçek kişi değildir.
/// </summary>
internal sealed record DemoSeedReport(
    bool Created,
    Guid BrandId,
    Uri BookingUrl,
    IReadOnlyList<DemoSeedReport.Login> Logins,
    int Branches,
    int Services,
    int Customers,
    IReadOnlyDictionary<DemoAppointmentOutcome, int> Appointments,
    int SmsCredits,
    DemoSeedReport.Extras? Dilim5 = null,
    DemoSeedReport.PlatformExtras? Dilim6 = null)
{
    public static DemoSeedReport Unchanged(Guid brandId, Uri bookingUrl) =>
        new(false, brandId, bookingUrl, AllLogins(), 0, 0, 0, new Dictionary<DemoAppointmentOutcome, int>(), 0);

    public async Task WriteAsync(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        await output.WriteLineAsync(Created
            ? $"Demo markası kuruldu: {DemoDataSet.BusinessName} ({BrandId})"
            : $"Demo markası zaten var, değişiklik yapılmadı ({BrandId}). Yeniden kurmak için: --role=seed --reset");
        await output.WriteLineAsync($"Rezervasyon sayfası: {BookingUrl} (bağlantı adı: {DemoDataSet.Slug})");
        await output.WriteLineAsync("Panel girişi (SMS kodu sahte sağlayıcıda: infra.dev_sms_log):");
        foreach (var login in Logins)
        {
            await output.WriteLineAsync($"  {login.Phone.Display}  {login.FullName} ({login.Role})");
        }

        await output.WriteLineAsync("Platform paneli girişi:");
        foreach (var user in DemoDataSet.PlatformUsers)
        {
            await output.WriteLineAsync($"  {PhoneNumber.Parse(user.Phone).Display}  {user.FullName} ({user.Role})");
        }

        if (Created)
        {
            await output.WriteLineAsync($"Şube {Branches}, hizmet {Services}, müşteri {Customers}, SMS kontörü {SmsCredits}");
            await output.WriteLineAsync("Randevular: " + string.Join(", ", Appointments.Select(a => $"{a.Key} {a.Value}")));
            if (Dilim5 is { } extras)
            {
                await extras.WriteAsync(output);
            }

            var iban = Iban.TryParse(DemoDataSet.BankAccount.Iban, out var parsed) ? parsed.Masked : string.Empty;
            await output.WriteLineAsync(
                $"Kapora: {DemoDataSet.DepositPolicy} (eşik {DemoDataSet.RiskyCustomerNoShowThreshold} gelmeme; {DemoDataSet.Customers[0]} ve "
                + $"{DemoDataSet.Customers[1]}), hesap {DemoDataSet.BankAccount.BankName} {iban}, havale süresi {DemoDataSet.BankAccount.TransferWindowMinutes} dk");
            if (Dilim6 is { } platform)
            {
                await platform.WriteAsync(output);
            }
        }
    }

    public static List<Login> AllLogins() =>
        [
            new(DemoDataSet.OwnerName, "Owner", DemoDataSet.OwnerPhone),
            .. DemoDataSet.Staff.Append(DemoDataSet.LateStaff).Select(s => new Login(s.FullName, s.Role, PhoneNumber.Parse(s.Phone))),
            .. DemoDataSet.ExtraBrands.Select(b => new Login($"{b.OwnerName}, {b.BusinessName}", "Owner", PhoneNumber.Parse(b.OwnerPhone))),
        ];

    /// <summary>Gün kapatma: gün, beklenen ve sayılan nakit (kuruş).</summary>
    internal sealed record CashDay(LocalDate Date, long ExpectedMinor, long CountedMinor);

    /// <summary>Dilim 5 verisi (T-114): paketler, tamamlanan randevular, kasa, değerlendirme, bekleme listesi, kapıda sıra.</summary>
    internal sealed record Extras(int Packages, int CustomerPackages, int Completed, CashDay? CashDayClose, int Reviews, int Waitlist, int WalkIns)
    {
        public async Task WriteAsync(TextWriter output)
        {
            await output.WriteLineAsync(
                $"Paket tanımı {Packages}, satılmış paket {CustomerPackages}, tamamlanan randevu {Completed} (tahsilat ve prim), değerlendirme {Reviews}, "
                + $"bekleme listesi {Waitlist}, kapıda sıra {WalkIns}");
            await output.WriteLineAsync(CashDayClose is { } c
                ? $"Kasa: {c.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)} kapatıldı (beklenen nakit {Lira(c.ExpectedMinor)}, sayılan {Lira(c.CountedMinor)})"
                : "Kasa: kapatılacak nakit günü yok");
        }

        private static string Lira(long minor) => (minor / 100m).ToString("N2", CultureInfo.GetCultureInfo("tr-TR")) + " TL";
    }

    /// <summary>Küçük demo markasının aboneliği: durum, fatura numarası (fiyat tanımlı değilse yok), banka hareketi sayısı.</summary>
    internal sealed record BrandSubscription(string BusinessName, string Status, string? InvoiceNumber, int BankTransactions);

    /// <summary>Dilim 6 verisi (T-131): abonelik durumları, havale bildirimleri, SMS adı başvurusu, Google Takvim, denetim kaydı.</summary>
    internal sealed record PlatformExtras(IReadOnlyList<BrandSubscription> Brands, bool CreditNotice, bool CalendarLinked, int AuditRows)
    {
        public async Task WriteAsync(TextWriter output)
        {
            await output.WriteLineAsync(
                $"Abonelik: {DemoDataSet.BusinessName} Trialing, "
                + string.Join(", ", Brands.Select(b => $"{b.BusinessName} {b.Status} (fatura {b.InvoiceNumber ?? "yok: plan fiyatı tanımlı değil"}, banka hareketi {b.BankTransactions})")));
            await output.WriteLineAsync(
                $"Onay kuyruğu: abonelik havalesi 1, kontör havalesi {(CreditNotice ? "1" : "0 (kontör paketi yok)")}, SMS gönderen adı 1 ({DemoDataSet.SmsSenderName}); denetim kaydı {AuditRows}");
            await output.WriteLineAsync(CalendarLinked
                ? $"Google Takvim: {DemoDataSet.OwnerName} bağlı (sahte sağlayıcı)"
                : "Google Takvim: bağlantı yazılmadı (sahte sağlayıcı kapalı veya Integrations:Calendar:TokenEncryptionKey tanımlı değil)");
        }
    }

    internal sealed record Login(string FullName, string Role, PhoneNumber Phone);
}
