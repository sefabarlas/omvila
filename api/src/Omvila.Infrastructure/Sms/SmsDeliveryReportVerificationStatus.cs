namespace Omvila.Infrastructure.Sms;

/// <summary>Teslim raporu doğrulama sonucu.</summary>
public enum SmsDeliveryReportVerificationStatus
{
    /// <summary>İmza geçerli, raporlar ayrıştırıldı.</summary>
    Valid,

    /// <summary>İmza yok veya tutmuyor (401).</summary>
    Unauthorized,

    /// <summary>İmza geçerli ama gövde anlaşılamadı (400).</summary>
    Malformed,
}
