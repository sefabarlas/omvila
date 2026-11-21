using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features;

/// <summary>
/// Sözleşme DepositView (randevu yan paneli AppointmentDetail.deposit) ve rezervasyon tarafında DepositView +
/// <see cref="Instructions"/> (PublicAppointment.deposit). İade notu burada yoktur; yalnız panel geçmişinde döner.
/// </summary>
internal sealed record DepositView(
    Money Required,
    string Status,
    string? TransferCode,
    Instant? CustomerReportedUtc,
    Instant? ReceivedUtc,
    Instant? RemindedUtc = null,
    Instant? RefundedUtc = null,
    Money? RefundedAmount = null,
    DepositInstructionsView? Instructions = null)
{
    /// <summary>Kapora istenmemişse null.</summary>
    public static DepositView? For(Appointment appointment, Currency currency, DepositInstructionsView? instructions = null)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        return appointment is { DepositRequiredMinor: { } required, DepositStatus: { } status }
            ? new DepositView(
                new Money(required, currency),
                status.ToString(),
                appointment.DepositTransferCode,
                appointment.DepositCustomerReportedUtc,
                appointment.DepositReceivedUtc,
                appointment.DepositRemindedUtc,
                appointment.DepositRefundedUtc,
                appointment.DepositRefundedAmountMinor is { } refunded ? new Money(refunded, currency) : null,
                instructions)
            : null;
    }
}
