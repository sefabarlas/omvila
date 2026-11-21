namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Denetim kaydı yazımı (alan-modeli.md § AuditLog; guvenlik-kvkk.md: anonimleştirme, dışa aktarma). Kayıt değiştirilemez ve
/// silinemez. Çağıranın isteğiyle aynı transaction'da yazılır: iş geri alınırsa kayıt da geri alınır.
/// </summary>
public interface IAuditLog
{
    /// <summary>Kaydı ekler; zaman enjekte edilen saatten alınır.</summary>
    /// <param name="entry">Kayıt.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task WriteAsync(AuditEntry entry, CancellationToken ct);
}
