namespace Omvila.Modules.Identity.Domain;

/// <summary>Platform rolü (alan-modeli.md § User.PlatformRole; roller-yetkiler.md § Platform rolleri; K3).</summary>
internal enum PlatformRole
{
    /// <summary>Yönetici.</summary>
    PlatformAdmin,

    /// <summary>Destek.</summary>
    PlatformSupport,

    /// <summary>Finans.</summary>
    PlatformFinance,
}
