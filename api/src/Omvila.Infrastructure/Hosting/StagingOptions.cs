namespace Omvila.Infrastructure.Hosting;

/// <summary>appsettings "Staging" bölümü: yalnız Staging ortamında anlamlıdır, diğer ortamlarda yok sayılır (T-73).</summary>
public sealed class StagingOptions
{
    public const string Section = "Staging";

    /// <summary>Sahte SMS ve push sağlayıcısına izin (ortam değişkeni <c>Staging__AllowFakeProviders</c>).</summary>
    public bool AllowFakeProviders { get; set; }
}
