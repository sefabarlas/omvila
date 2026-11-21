namespace Omvila.Modules.Platform.Contracts;

/// <summary>Denetim kaydında işlemi yapan (alan-modeli.md § AuditLog.ActorType).</summary>
public enum AuditActorType
{
    /// <summary>Salon kullanıcısı.</summary>
    User,

    /// <summary>OMVILA iç ekip kullanıcısı.</summary>
    PlatformUser,

    /// <summary>Müşteri.</summary>
    Customer,

    /// <summary>Sistem işi.</summary>
    System,
}
