namespace Omvila.Modules.Tenancy.Domain;

/// <summary>alan-modeli.md § WorkingHoursOverride.Kind.</summary>
internal enum OverrideKind
{
    /// <summary>Resmi tatil: kapalı veya özel saatle açık.</summary>
    Holiday,

    /// <summary>Kapanış (izin günü, tadilat): her zaman kapalı.</summary>
    Closure,

    /// <summary>"Yalnızca bu gün için aç": kapalı güne saat.</summary>
    ExtraOpening,

    /// <summary>Erken kapanış veya kısa gün: saatle açık.</summary>
    ShortDay,
}
