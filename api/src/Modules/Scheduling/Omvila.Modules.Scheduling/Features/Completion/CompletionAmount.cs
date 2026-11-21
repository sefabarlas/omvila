namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>Sözleşme Money (CompletionPayment.amount).</summary>
/// <param name="AmountMinor">Kuruş.</param>
/// <param name="Currency">Para birimi (TRY).</param>
internal sealed record CompletionAmount(long? AmountMinor, string? Currency);
