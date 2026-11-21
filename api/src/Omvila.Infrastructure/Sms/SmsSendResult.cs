namespace Omvila.Infrastructure.Sms;

/// <param name="Succeeded">Sağlayıcı mesajı kabul etti.</param>
/// <param name="ProviderMessageId">Sağlayıcının mesaj kimliği; teslim raporu bununla eşlenir.</param>
/// <param name="FailureReason">Kısa hata kodu (kişisel veri içermez).</param>
/// <param name="IsPermanent">Tekrar denemenin anlamı olmayan hata (ör. geçersiz numara): mesaj doğrudan <c>Failed</c> olur.</param>
public sealed record SmsSendResult(bool Succeeded, string? ProviderMessageId, string? FailureReason, bool IsPermanent = false);
