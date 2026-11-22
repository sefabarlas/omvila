namespace Omvila.Infrastructure.Files;

/// <summary>appsettings "FileStorage".</summary>
public sealed class FileStorageOptions
{
    public const string Section = "FileStorage";

    /// <summary>Kök klasör (göreli ise uygulama çalışma dizinine göre).</summary>
    public string RootPath { get; set; } = string.Empty;

    internal bool IsValid() => !string.IsNullOrWhiteSpace(RootPath);
}
