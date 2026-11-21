using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Randevu SMS'lerinin ortak işleri (bildirimler.md § Müşteri olayları): kiracı bağlamı, değişkenler (salon adı, şube
/// saat diliminde tarih/saat, /r/ bağlantısı), müşterinin "Randevu bilgilendirme SMS'leri" tercihi ve hatırlatma planı.
/// Telefon yalnız outbox satırına yazılır; günlüğe yazılmaz.
/// </summary>
internal sealed class AppointmentSms(
    IDbSession session,
    IAppointmentQueries appointments,
    IPublicBookingQueries booking,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    INotificationOutbox outbox,
    IClock clock)
{
    /// <summary>İşleyici kendi transaction'ında çalışır; kiracı bağlamı olaydaki markadan kurulur.</summary>
    public Task EnterBrandAsync(Guid brandId, CancellationToken ct) => session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);

    /// <summary>Randevu, şube veya marka bulunamazsa null (silinmiş / başka marka): bildirim yazılmaz.</summary>
    public async Task<AppointmentSmsContext?> LoadAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        await EnterBrandAsync(brandId, ct);
        var appointment = await appointments.GetNotificationInfoAsync(brandId, appointmentId, ct);
        if (appointment is null)
        {
            return null;
        }

        var branch = await booking.GetBranchCardAsync(brandId, appointment.BranchId, ct);
        if (branch is null)
        {
            return null;
        }

        var cards = await customers.GetDisplayInfoAsync(brandId, [appointment.BrandCustomerId], ct);
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(branch.TimeZoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        return new AppointmentSmsContext(brandId, appointment, branch, cards.GetValueOrDefault(appointment.BrandCustomerId), zone);
    }

    /// <summary>
    /// Müşteriye SMS yazılabilir mi: kart var, "Randevu bilgilendirme SMS'leri gönderilsin" açık (T-46 (3)), numara
    /// anonimleştirilmemiş ve geçerli.
    /// </summary>
    public static bool CanText(AppointmentSmsContext context) =>
        context.Customer is { AppointmentSmsEnabled: true, PhoneMasked: not null } customer
        && PhoneNumber.TryParse(customer.PhoneE164, out _);

    public Task EnqueueAsync(
        AppointmentSmsContext context,
        string eventCode,
        IReadOnlyDictionary<string, string> values,
        CancellationToken ct,
        Instant? scheduledUtc = null,
        string? variant = null,
        Guid? customerId = null) =>
        outbox.EnqueueSmsAsync(
            new SmsNotification(
                eventCode,
                context.Customer!.PhoneE164!,
                values,
                context.BrandId,
                NotificationReference.ForAppointment(context.Appointment.AppointmentId),
                scheduledUtc,
                variant,
                customerId),
            ct);

    /// <summary>"Değişiklik ve iptal" bağlantısı (/r/{kod}); kod yoksa null.</summary>
    public string? ManageLink(AppointmentSmsContext context) =>
        context.Appointment.ManageAccessCode is { } code ? SmsFormat.Link(booking.AppointmentPageUrl(code)) : null;

    /// <summary>Rezervasyon sayfası bağlantısı (iptal mesajında "Başka bir saat seçin").</summary>
    public string BookingLink(AppointmentSmsContext context) => SmsFormat.Link(booking.BookingPageUrl(context.Branch.BrandSlug));

    public static string Date(AppointmentSmsContext context, Instant at) => SmsFormat.Date(at.InZone(context.Zone).Date);

    public static string Time(AppointmentSmsContext context, Instant at) => SmsFormat.Time(at.InZone(context.Zone).TimeOfDay);

    /// <summary>
    /// ScheduleReminders (durum-makinesi.md § Sistem işleri): önce randevunun bekleyen hatırlatmaları düşer, sonra güncel
    /// başlangıca göre yeniden planlanır (olay tekrar veya sırasız işlense de tek takım hatırlatma kalır). Yalnız onaylı
    /// randevuda ve müşteriye SMS yazılabiliyorsa; hatırlatma işlem mesajıdır (H-01), salt okunur abonelikte de gider
    /// (durum-makinesi.md § 8).
    /// </summary>
    public async Task RescheduleRemindersAsync(AppointmentSmsContext context, CancellationToken ct)
    {
        await CancelRemindersAsync(context.Appointment.AppointmentId, ct);
        if (!context.IsConfirmed || !CanText(context) || ManageLink(context) is not { } link)
        {
            return;
        }

        // Kural: sureler-ve-ayarlar.md ReminderOffsetsMinutes — markanın bildirim ayarı (kanonik uç notification-settings, T-62 (4)).
        var offsets = (await tenants.GetNotificationSettingsAsync(context.BrandId, ct))?.ReminderOffsetsMinutes ?? [];
        var start = context.Appointment.StartUtc;
        foreach (var reminder in ReminderPlanner.Plan(start, clock.GetCurrentInstant(), context.Zone, offsets))
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["saat"] = Time(context, start),
                ["salon"] = context.Branch.BrandName,
                ["bağlantı"] = link,
            };
            await EnqueueAsync(context, NotificationEvents.AppointmentReminder, values, ct, reminder.ScheduledUtc, reminder.Variant);
        }
    }

    /// <summary>
    /// Talep iptal edildi (A4 red, A5 süre dolumu): bekleyen hatırlatmalar düşer, müşteriye "talep iptal edildi" metni ve
    /// rezervasyon sayfası bağlantısı yazılır. Tarih ve saat iptal edilen talebinkidir.
    /// </summary>
    public async Task NotifyRequestCancelledAsync(Guid brandId, Guid appointmentId, Instant startUtc, string eventCode, CancellationToken ct)
    {
        var context = await LoadAsync(brandId, appointmentId, ct);
        await CancelRemindersAsync(appointmentId, ct);
        if (context is null || !CanText(context))
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["tarih"] = Date(context, startUtc),
            ["saat"] = Time(context, startUtc),
            ["rezervasyon bağlantısı"] = BookingLink(context),
        };
        await EnqueueAsync(context, eventCode, values, ct);
    }

    /// <summary>"Değerlendirme bağlantısı" (/d/{kod}; review.invite).</summary>
    public string ReviewLink(string accessCode) => SmsFormat.Link(booking.ReviewPageUrl(accessCode));

    /// <summary>
    /// Randevu iptal edildi (A6, A7, A12): bekleyen hatırlatmalar ve değerlendirme daveti düşer (bildirimler.md § İptal). Kiracı
    /// bağlamı kurulmuş olmalı.
    /// </summary>
    public async Task CancelPendingOnCancellationAsync(Guid appointmentId, CancellationToken ct)
    {
        await CancelRemindersAsync(appointmentId, ct);
        await outbox.CancelPendingAsync(NotificationEvents.ReviewInvite, NotificationReference.ForAppointment(appointmentId), ct);
    }

    /// <summary>Randevunun bekleyen hatırlatmalarını düşürür (bildirimler.md § İptal). Kiracı bağlamı kurulmuş olmalı.</summary>
    public Task<int> CancelRemindersAsync(Guid appointmentId, CancellationToken ct) =>
        outbox.CancelPendingAsync(NotificationEvents.AppointmentReminder, NotificationReference.ForAppointment(appointmentId), ct);
}
