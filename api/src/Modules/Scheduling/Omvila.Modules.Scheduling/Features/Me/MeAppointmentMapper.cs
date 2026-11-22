using System.Globalization;
using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>
/// Randevu → sözleşme MeAppointmentSummary / MeAppointment. Tutarlar ve son süreler SMS bağlantısındaki "Randevum" ile aynı kuralla
/// (PublicAppointmentMapper): son süreler yalnız onaylı randevuda, sınır tam eşitlikte serbest. Değerlendirme yalnız tamamlanmış ve
/// değerlendirilmemiş randevuda (KK8). Personel adı rezervasyon sayfasındaki görünen addır.
/// </summary>
internal static class MeAppointmentMapper
{
    public static MeAppointmentSummary Summary(Appointment appointment, PublicBranchCard card, MeAppointmentContext context)
    {
        var currency = CurrencyOf(appointment);
        var review = ReviewOf(appointment, context);
        return new MeAppointmentSummary(
            appointment.Id,
            BrandOf(appointment, card),
            new MeAppointmentView.BranchRef(card.BranchId, card.BranchName, card.District),
            card.TimeZoneId,
            appointment.StartUtc,
            appointment.EndUtc,
            appointment.Status.ToString(),
            appointment.DisplayStatus.ToString(),
            appointment.Status == AppointmentStatus.PendingApproval ? appointment.HoldExpiresUtc : null,
            appointment.DepositStatus?.ToString(),
            ItemsOf(appointment, currency, context),
            new Money(appointment.TotalAmountMinor, currency),
            new Money(appointment.AmountDueMinor, currency),
            review,
            appointment.Status == AppointmentStatus.Completed && review is null,
            context.Rebookable.Contains(appointment.Id));
    }

    // Kural: durum-makinesi.md A6, A14 (son süreler yalnız onaylı randevuda); sözleşme meGetAppointment
    public static MeAppointmentView Detail(
        Appointment appointment, PublicBranchCard card, BrandAppointmentSettings settings, MeAppointmentContext context, ActiveBankAccount? account, Instant now)
    {
        var currency = CurrencyOf(appointment);
        var active = appointment.Status is AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed;
        var confirmed = appointment.Status == AppointmentStatus.Confirmed;
        Instant? cancelDeadline = confirmed ? appointment.StartUtc - Duration.FromHours(settings.CancellationCutoffHours) : null;
        Instant? rescheduleDeadline = confirmed ? appointment.StartUtc - Duration.FromHours(settings.RescheduleCutoffHours) : null;
        var review = ReviewOf(appointment, context);
        return new MeAppointmentView(
            appointment.Id,
            BrandOf(appointment, card),
            new MeAppointmentView.BranchDetail(card.BranchId, card.BranchName, card.AddressLine, card.District, card.Phone, card.Latitude, card.Longitude),
            card.TimeZoneId,
            appointment.StartUtc,
            appointment.EndUtc,
            appointment.Status.ToString(),
            appointment.DisplayStatus.ToString(),
            appointment.Status == AppointmentStatus.PendingApproval ? appointment.HoldExpiresUtc : null,
            ItemsOf(appointment, currency, context),
            new Money(appointment.TotalAmountMinor, currency),
            new Money(appointment.AmountDueMinor, currency),
            DepositView.For(appointment, currency, DepositInstructionsView.For(appointment, currency, account)),
            active && (cancelDeadline is null || now <= cancelDeadline),
            cancelDeadline,
            active && (rescheduleDeadline is null || now <= rescheduleDeadline),
            rescheduleDeadline,
            appointment.Status == AppointmentStatus.Completed && review is null,
            context.Rebookable.Contains(appointment.Id),
            review,
            card.PreAppointmentNote,
            appointment.CustomerNote,
            appointment is { Status: AppointmentStatus.Cancelled, CancelledUtc: { } cancelledUtc, CancelledBy: { } by, CancellationReason: { } reason }
                ? new MeAppointmentView.CancellationRef(cancelledUtc, by.ToString(), reason.ToString())
                : null,
            appointment.Version.ToString(CultureInfo.InvariantCulture));
    }

    private static MeAppointmentView.BrandRef BrandOf(Appointment appointment, PublicBranchCard card) =>
        new(appointment.BrandId, card.BrandName, card.BrandSlug, card.LogoUrl);

    private static MeAppointmentView.ReviewRef? ReviewOf(Appointment appointment, MeAppointmentContext context) =>
        context.Reviews.TryGetValue(appointment.Id, out var r) ? new MeAppointmentView.ReviewRef(r.Rating, r.CreatedUtc) : null;

    private static List<MeAppointmentView.Item> ItemsOf(Appointment appointment, Currency currency, MeAppointmentContext context) =>
        [.. appointment.Items
            .Where(MeAppointmentReader.Visible(appointment))
            .OrderBy(i => i.SeqNo)
            .Select(i => new MeAppointmentView.Item(
                i.ServiceId,
                i.ServiceNameSnapshot,
                i.StaffId,
                i.StaffId is { } staffId ? context.Staff.GetValueOrDefault(staffId)?.FullName : null,
                i.StartUtc,
                i.EndUtc,
                new Money(i.PriceMinor, currency),
                i.CustomerPackageItemId is not null))];

    private static Currency CurrencyOf(Appointment appointment) =>
        Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
}
