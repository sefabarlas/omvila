namespace Omvila.Infrastructure.Sms;

public sealed record SmsSendResult(bool Succeeded, string? ProviderMessageId, string? FailureReason);
