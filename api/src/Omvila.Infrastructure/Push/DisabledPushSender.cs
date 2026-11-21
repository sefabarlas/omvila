namespace Omvila.Infrastructure.Push;

/// <summary>Push yapılandırılmamış: her gönderim <see cref="PushSendStatus.Disabled"/> döner.</summary>
internal sealed class DisabledPushSender : IPushSender
{
    public Task<PushSendResult> SendAsync(PushMessage message, CancellationToken ct) =>
        Task.FromResult(new PushSendResult(PushSendStatus.Disabled, FailureReason: "push_disabled"));
}
