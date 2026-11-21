namespace Omvila.Infrastructure.Sms;

/// <summary>Sağlayıcı teslim raporunun sağlayıcıdan bağımsız hâli (sözleşme webhookSmsDeliveryReport).</summary>
/// <param name="ProviderMessageId">Gönderimde sağlayıcının verdiği kimlik.</param>
/// <param name="Delivered">true: teslim edildi; false: teslim edilemedi.</param>
/// <param name="FailureReason">Teslim edilemediyse sağlayıcının kısa hata kodu.</param>
public sealed record SmsDeliveryReport(string ProviderMessageId, bool Delivered, string? FailureReason);
