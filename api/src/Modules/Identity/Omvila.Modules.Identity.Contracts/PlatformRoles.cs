namespace Omvila.Modules.Identity.Contracts;

/// <summary>Platform rolleri (sözleşme PlatformRole; roller-yetkiler.md § Platform rolleri; K3). Jetonda platform_role iddiası.</summary>
public static class PlatformRoles
{
    /// <summary>Yönetici.</summary>
    public const string Admin = "PlatformAdmin";

    /// <summary>Destek.</summary>
    public const string Support = "PlatformSupport";

    /// <summary>Finans.</summary>
    public const string Finance = "PlatformFinance";
}
