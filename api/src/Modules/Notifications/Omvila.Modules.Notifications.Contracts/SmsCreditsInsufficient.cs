using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Kontör yetmediği için SMS atlandı (alan-modeli.md § CreditTransaction: "salona uyarı üretilir"; dilim-03 kriter 7).
/// Gönderim işinin her çalışmasında marka başına bir kez yazılır; uygulama içi "Kontör az" uyarısına çevrilir.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="SkippedCount">Bu çalışmada atlanan mesaj sayısı.</param>
/// <param name="Balance">Güncel bakiye.</param>
public sealed record SmsCreditsInsufficient(Guid BrandId, int SkippedCount, int Balance) : IIntegrationEvent;
