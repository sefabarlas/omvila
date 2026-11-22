namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeAccountDeletionConfirm (meRequestAccountDeletionOtp'tan gelen kod).</summary>
internal sealed record MeAccountDeletionConfirm(Guid? OtpId, string? Code);
