namespace Omvila.Modules.Platform.Contracts;

/// <summary>Denetim kaydı eylemi (alan-modeli.md § AuditLog.Action).</summary>
public enum AuditAction
{
    /// <summary>Oluşturma.</summary>
    Create,

    /// <summary>Değişiklik.</summary>
    Update,

    /// <summary>Silme.</summary>
    Delete,

    /// <summary>Görüntüleme (platform kullanıcısı).</summary>
    View,

    /// <summary>Onay.</summary>
    Approve,

    /// <summary>Havale eşleştirme.</summary>
    Match,

    /// <summary>Dışa aktarma.</summary>
    Export,

    /// <summary>KVKK anonimleştirme.</summary>
    Anonymize,
}
