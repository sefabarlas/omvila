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
    DemoSeedReport.Extras? Dilim5 = null)
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
        }
    }

    public static List<Login> AllLogins() =>
        [new(DemoDataSet.OwnerName, "Owner", DemoDataSet.OwnerPhone), .. DemoDataSet.Staff.Select(s => new Login(s.FullName, s.Role, PhoneNumber.Parse(s.Phone)))];

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

    internal sealed record Login(string FullName, string Role, PhoneNumber Phone);
}
