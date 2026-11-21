namespace Omvila.Modules.Scheduling.Features.Deposits;

/// <summary>Sözleşme Money (DepositRefundRequest.amount).</summary>
/// <param name="AmountMinor">Kuruş.</param>
/// <param name="Currency">Para birimi (TRY).</param>
internal sealed record DepositRefundAmount(long? AmountMinor, string? Currency);
