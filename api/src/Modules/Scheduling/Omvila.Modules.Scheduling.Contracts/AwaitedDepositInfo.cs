using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Beklenen kapora (deposit.awaiting SMS'i). IBAN ve alıcı randevuda tutulmaz; Tenancy'nin aktif hesabından okunur (V10).</summary>
/// <param name="Amount">İstenen kapora.</param>
/// <param name="TransferCode">Havale açıklama kodu.</param>
/// <param name="DueUtc">Kapora bu anda gelmemişse randevu iptal olur (HoldExpiresUtc).</param>
public sealed record AwaitedDepositInfo(SharedKernel.Money Amount, string TransferCode, Instant DueUtc);
