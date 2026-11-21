using System.Globalization;
using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Randevu → sözleşme PublicAppointment (oluşturma yanıtı; SMS bağlantısı uçları da kullanır). İptal ve saat değişikliği
/// son süresi yalnız onaylı randevuda uygulanır; onay bekleyen talepte sınır yoktur (durum-makinesi.md A6, A14; sınır tam
/// eşitlikte serbest). Personel adı rezervasyon sayfasındaki görünen addır. Kapora bekleniyorsa havale talimatı güncel aktif
/// hesaptan (V10) eklenir; "Salonda ödenecek" toplam − paketten düşülen kalemler − alınmış kapora.
/// </summary>
internal static class PublicAppointmentMapper
{
    public static PublicAppointmentView Map(
        Appointment appointment,
        string accessCode,
        Uri manageUrl,
        PublicBranchCard card,
        BrandAppointmentSettings settings,
        IReadOnlyDictionary<Guid, StaffCard> staff,
        ActiveBankAccount? account,
        Instant now)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(staff);
        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var active = appointment.Status is AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed;
        var confirmed = appointment.Status == AppointmentStatus.Confirmed;
        Instant? cancelDeadline = confirmed ? appointment.StartUtc - Duration.FromHours(settings.CancellationCutoffHours) : null;
        Instant? rescheduleDeadline = confirmed ? appointment.StartUtc - Duration.FromHours(settings.RescheduleCutoffHours) : null;
        var total = new Money(appointment.TotalAmountMinor, currency);
        return new PublicAppointmentView(
            appointment.Id,
            accessCode,
            manageUrl,
            appointment.Status.ToString(),
            appointment.DisplayStatus.ToString(),
            new PublicAppointmentView.BrandView(card.BrandName, card.BrandSlug, card.LogoUrl),
            new PublicAppointmentView.BranchView(card.BranchId, card.BranchName, card.AddressLine, card.District, card.Phone, card.Latitude, card.Longitude),
            appointment.StartUtc,
            appointment.EndUtc,
            card.TimeZoneId,
            appointment.Status == AppointmentStatus.PendingApproval ? appointment.HoldExpiresUtc : null,
            [.. appointment.Items
                .Where(i => i.Status != AppointmentItemStatus.Cancelled || appointment.Status == AppointmentStatus.Cancelled)
                .OrderBy(i => i.SeqNo)
                .Select(i => new PublicAppointmentView.ItemView(
                    i.ServiceNameSnapshot,
                    i.StaffId is { } staffId ? staff.GetValueOrDefault(staffId)?.FullName : null,
                    i.StartUtc,
                    i.EndUtc,
                    new Money(i.PriceMinor, currency),
                    i.CustomerPackageItemId is not null))],
            total,
            // "Salonda ödenecek": toplam − paketten düşülenler − alınmış kapora.
            new Money(appointment.AmountDueMinor, currency),
            DepositView.For(appointment, currency, DepositInstructionsView.For(appointment, currency, account)),
            active && (cancelDeadline is null || now <= cancelDeadline),
            cancelDeadline,
            active && (rescheduleDeadline is null || now <= rescheduleDeadline),
            rescheduleDeadline,
            card.PreAppointmentNote,
            appointment is { Status: AppointmentStatus.Cancelled, CancelledUtc: { } cancelledUtc, CancelledBy: { } by, CancellationReason: { } reason }
                ? new PublicAppointmentView.CancellationView(cancelledUtc, by.ToString(), reason.ToString())
                : null,
            appointment.Version.ToString(CultureInfo.InvariantCulture));
    }
}
