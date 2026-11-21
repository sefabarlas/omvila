namespace Omvila.Modules.Catalog.Contracts;

/// <summary><see cref="IServiceDeposits.ResolveAsync"/> girdisi.</summary>
/// <param name="IsOnlineChannel">Randevu kanalı Online mı (v1'de panel, telefon ve kapıda sıra kapora istemez; S-08).</param>
/// <param name="CustomerNoShowCount">Markadaki müşteri kartının gelmeme sayısı (riskli müşteri kuralı).</param>
/// <param name="Items">Kalemler.</param>
public sealed record ServiceDepositRequest(bool IsOnlineChannel, int CustomerNoShowCount, IReadOnlyList<ServiceDepositItem> Items);
