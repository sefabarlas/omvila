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
    int SmsCredits)
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
            var iban = Iban.TryParse(DemoDataSet.BankAccount.Iban, out var parsed) ? parsed.Masked : string.Empty;
            await output.WriteLineAsync(
                $"Kapora: {DemoDataSet.DepositPolicy} (eşik {DemoDataSet.RiskyCustomerNoShowThreshold} gelmeme; {DemoDataSet.Customers[0]} ve "
                + $"{DemoDataSet.Customers[1]}), hesap {DemoDataSet.BankAccount.BankName} {iban}, havale süresi {DemoDataSet.BankAccount.TransferWindowMinutes} dk");
        }
    }

    public static List<Login> AllLogins() =>
        [new(DemoDataSet.OwnerName, "Owner", DemoDataSet.OwnerPhone), .. DemoDataSet.Staff.Select(s => new Login(s.FullName, s.Role, PhoneNumber.Parse(s.Phone)))];

    internal sealed record Login(string FullName, string Role, PhoneNumber Phone);
}
