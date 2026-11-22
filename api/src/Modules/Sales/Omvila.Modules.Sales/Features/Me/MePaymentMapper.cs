using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Tahsilat kaydı veya bildirilmiş kapora → sözleşme MePaymentRow.</summary>
internal static class MePaymentMapper
{
    public const string AwaitingConfirmation = "AwaitingConfirmation";

    /// <summary>v1'de bir tahsilat tek hedefe tahsis edilir; tahsissiz kayıt hedefsizdir (PaymentRows ile aynı kural).</summary>
    public static PaymentAllocation? Target(Payment payment) => payment.Allocations.OrderBy(a => a.Id).FirstOrDefault();

    // Kural: sözleşme MePaymentRow (Deposit, AppointmentPayment, PackagePurchase; Recorded / Refunded / PartiallyRefunded)
    public static MePaymentRow FromPayment(
        Payment payment,
        CustomerBrandCard brand,
        string timeZone,
        IReadOnlyDictionary<Guid, AppointmentPaymentContext> contexts,
        IReadOnlyDictionary<Guid, CustomerPackage> packages)
    {
        var currency = Enum.TryParse<Currency>(payment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var target = Target(payment);
        var package = target is { TargetType: AllocationTargetType.CustomerPackage } && packages.TryGetValue(target.TargetId, out var p) ? p : null;
        var appointmentId = target is { TargetType: AllocationTargetType.Appointment } ? target.TargetId : (Guid?)null;
        var context = appointmentId is { } id && contexts.TryGetValue(id, out var found) ? found : null;
        var kind = payment.IsDeposit ? "Deposit" : package is not null || target is { TargetType: AllocationTargetType.CustomerPackage } ? "PackagePurchase" : "AppointmentPayment";
        return new MePaymentRow(
            payment.Id,
            kind,
            payment.Status.ToString(),
            new MeBrandRef(brand.Id, brand.Name, brand.Slug, brand.LogoUrl),
            new Money(payment.AmountMinor, currency),
            payment.Method.ToString(),
            payment.PaidUtc,
            timeZone,
            payment.RefundedAmountMinor > 0 ? new Money(payment.RefundedAmountMinor, currency) : null,
            appointmentId,
            context?.StartUtc,
            context?.ServiceNames,
            package?.PackageNameSnapshot,
            package?.TotalSessions);
    }

    /// <summary>"Salon onayı bekleniyor": tahsilat kaydı değildir, kimlik randevunundur (sözleşme MePaymentStatus).</summary>
    public static MePaymentRow FromReported(ReportedDepositInfo reported, CustomerBrandCard brand, string timeZone) => new(
        reported.AppointmentId,
        "Deposit",
        AwaitingConfirmation,
        new MeBrandRef(brand.Id, brand.Name, brand.Slug, brand.LogoUrl),
        reported.Amount,
        nameof(PaymentMethod.Transfer),
        reported.ReportedUtc,
        timeZone,
        null,
        reported.AppointmentId,
        reported.StartUtc,
        reported.ServiceNames,
        null,
        null);
}
