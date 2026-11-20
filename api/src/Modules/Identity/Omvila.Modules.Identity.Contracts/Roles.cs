namespace Omvila.Modules.Identity.Contracts;

/// <summary>Salon sistem rolleri (docs/alan/roller-yetkiler.md § Salon rolleri). v1'de markaya özel rol yoktur.</summary>
public static class Roles
{
    /// <summary>İşletme sahibi; bütün şubeler.</summary>
    public const string Owner = "Owner";

    /// <summary>Şube yöneticisi; atandığı şubeler.</summary>
    public const string BranchManager = "BranchManager";

    /// <summary>Personel; atandığı şubeler, çoğu işlemde yalnız kendi kayıtları.</summary>
    public const string Staff = "Staff";
}
