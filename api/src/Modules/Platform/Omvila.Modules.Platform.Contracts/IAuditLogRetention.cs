using NodaTime;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Denetim kaydı saklama (sureler-ve-ayarlar.md AuditLogRetentionYears 2; T-121). Kayıtlar silinmez ve eylem, varlık, zaman
/// değişmez; süresi geçen kayıtların kişisel ayrıntıları (IP adresi, istemci) boşaltılır. <c>changes</c> zaten kişisel veri
/// taşımaz. Platform bağlamında (ApplyRetention işi) çağrılır.
/// </summary>
public interface IAuditLogRetention
{
    /// <summary>Verilen andan önceki kayıtlarda IP ve istemci bilgisini boşaltır; etkilenen kayıt sayısını döner.</summary>
    /// <param name="before">Bu andan eski kayıtlar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> ClearPersonalDetailsBeforeAsync(Instant before, CancellationToken ct);
}
