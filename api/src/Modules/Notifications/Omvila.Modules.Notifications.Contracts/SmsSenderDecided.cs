using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Gönderen adı onaylandı veya reddedildi (markaya bildirim paneli kaydı ve e-posta; T-124, T-132).</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="Name">Başvurulan ad.</param>
/// <param name="Approved">Onay mı.</param>
/// <param name="Reason">Ret nedeni.</param>
public sealed record SmsSenderDecided(Guid BrandId, string Name, bool Approved, string? Reason) : IIntegrationEvent;
