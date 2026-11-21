using Microsoft.AspNetCore.Http;

namespace Omvila.Infrastructure.Sms;

/// <summary>
/// Sağlayıcıya özgü teslim raporu adaptörü (T-62 (8): webhook OMVILA kimliği taşımaz, imza adaptörde doğrulanır).
/// Sağlayıcı koduyla (yol parametresi, ör. "fake") keyed servis olarak kaydedilir; kayıtlı olmayan kod 404'tür.
/// </summary>
public interface ISmsDeliveryReportVerifier
{
    /// <summary>İmzayı gövdenin ham baytları üzerinden sabit zamanlı doğrular ve raporları ayrıştırır.</summary>
    /// <param name="headers">İstek başlıkları.</param>
    /// <param name="body">Ham gövde.</param>
    SmsDeliveryReportVerification Verify(IHeaderDictionary headers, ReadOnlySpan<byte> body);
}
