namespace Omvila.Infrastructure.Push;

/// <param name="Status">Sonuç.</param>
/// <param name="ProviderMessageId">Sağlayıcının mesaj kimliği.</param>
/// <param name="FailureReason">Kısa hata kodu (kişisel veri ve jeton içermez).</param>
public sealed record PushSendResult(PushSendStatus Status, string? ProviderMessageId = null, string? FailureReason = null);
