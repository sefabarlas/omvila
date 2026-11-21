using NodaTime;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Kapora SMS'leri (bildirimler.md deposit.awaiting, deposit.not_received). Kapora mesajı her zaman SMS'tir (push denenmez)
/// ve hemen gider (sessiz saatte kaydırılmaz). IBAN, alıcı ve havale süresi randevuda tutulmaz; güncel aktif hesaptan
/// okunur (V10). IBAN ve alıcı adı yalnız outbox satırına yazılır, günlüğe yazılmaz.
/// </summary>
internal sealed class DepositSms(AppointmentSms sms, IBrandBankAccounts bankAccounts, IClock clock)
{
    /// <summary>
    /// "{salon}: {tarih} {saat} için {tutar} TL kaporayı {süre} içinde gönderin. IBAN …" (oluşturma, A13 ve "Hatırlat"); süre
    /// kalan süredir. Hesap pasifleştiyse yazılmaz. Çağıran <see cref="AppointmentSms.CanText"/>'i denetlemiş olmalı.
    /// </summary>
    // Kural: bildirimler.md deposit.awaiting; V2 (aşımda alıcı adı kırpılır, IBAN boşluksuz)
    public async Task<bool> EnqueueAwaitingAsync(AppointmentSmsContext context, AwaitedDepositInfo deposit, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(deposit);
        if (await bankAccounts.GetActiveAsync(context.BrandId, ct) is not { } account)
        {
            return false;
        }

        var start = context.Appointment.StartUtc;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["tarih"] = AppointmentSms.Date(context, start),
            ["saat"] = AppointmentSms.Time(context, start),
            ["tutar"] = SmsFormat.Amount(deposit.Amount.AmountMinor),
            ["süre"] = SmsFormat.Duration(deposit.DueUtc - clock.GetCurrentInstant()),
            ["iban"] = account.Iban.Value,
            ["alıcı"] = account.AccountHolderName,
            ["kod"] = deposit.TransferCode,
        };
        await sms.EnqueueAsync(context, NotificationEvents.DepositAwaiting, values, ct);
        return true;
    }

    /// <summary>
    /// A5 (kapora vardı): "{salon}: {tutar} TL kapora {süre} içinde gelmediği için {tarih} {saat} randevunuz iptal edildi…".
    /// Süre markanın havale süresidir (aktif hesap; hesap pasifleştiyse varsayılan TransferWindowMinutes; T-83). Tarih ve saat
    /// iptal edilen talebinkidir.
    /// </summary>
    // Kural: bildirimler.md deposit.not_received; durum-makinesi.md A5
    public async Task EnqueueNotReceivedAsync(AppointmentSmsContext context, Instant startUtc, long amountMinor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        var window = (await bankAccounts.GetActiveAsync(context.BrandId, ct))?.TransferWindowMinutes ?? PlatformDefaults.TransferWindowMinutes;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["tutar"] = SmsFormat.Amount(amountMinor),
            ["süre"] = SmsFormat.Duration(Duration.FromMinutes(window)),
            ["tarih"] = AppointmentSms.Date(context, startUtc),
            ["saat"] = AppointmentSms.Time(context, startUtc),
        };
        await sms.EnqueueAsync(context, NotificationEvents.DepositNotReceived, values, ct);
    }
}
