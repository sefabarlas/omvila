namespace Omvila.Infrastructure.Sms;

/// <param name="Status">Sonuç.</param>
/// <param name="Reports">Geçerliyse raporlar (bir istekte birden çok olabilir).</param>
public sealed record SmsDeliveryReportVerification(SmsDeliveryReportVerificationStatus Status, IReadOnlyList<SmsDeliveryReport> Reports)
{
    public static SmsDeliveryReportVerification Unauthorized { get; } = new(SmsDeliveryReportVerificationStatus.Unauthorized, []);

    public static SmsDeliveryReportVerification Malformed { get; } = new(SmsDeliveryReportVerificationStatus.Malformed, []);

    public static SmsDeliveryReportVerification Valid(IReadOnlyList<SmsDeliveryReport> reports) => new(SmsDeliveryReportVerificationStatus.Valid, reports);
}
