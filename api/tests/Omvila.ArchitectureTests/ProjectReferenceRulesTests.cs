using System.Xml.Linq;

namespace Omvila.ArchitectureTests;

// Kural: docs/mimari/mimari.md § Katman kuralları (3: başka modülün yalnız .Contracts projesine başvurulur)
public sealed class ProjectReferenceRulesTests
{
    private static readonly string SrcDir = Path.Combine(ApiRoot(), "src");

    [Fact]
    public void SharedKernel_ReferencesNoOtherProject() =>
        ProjectReferences(Path.Combine(SrcDir, "Omvila.SharedKernel", "Omvila.SharedKernel.csproj")).ShouldBeEmpty();

    [Fact]
    public void Modules_ReferenceOtherModulesOnlyThroughContracts()
    {
        var violations = new List<string>();
        foreach (var project in ModuleProjects())
        {
            var ownModule = ModuleName(project);
            var isContracts = Path.GetFileNameWithoutExtension(project).EndsWith(".Contracts", StringComparison.Ordinal);

            foreach (var reference in ProjectReferences(project))
            {
                var name = Path.GetFileNameWithoutExtension(reference);
                if (!IsAllowed(name, ownModule, isContracts))
                {
                    violations.Add($"{Path.GetFileName(project)} → {name}");
                }
            }
        }

        violations.ShouldBeEmpty();
    }

    private static bool IsAllowed(string reference, string ownModule, bool fromContracts)
    {
        // Modüller Host'a hiç, Contracts projeleri Infrastructure'a başvuramaz.
        if (string.Equals(reference, "Omvila.Host", StringComparison.Ordinal))
        {
            return false;
        }

        if (reference.StartsWith("Omvila.Infrastructure", StringComparison.Ordinal))
        {
            return !fromContracts;
        }

        if (!reference.StartsWith("Omvila.Modules.", StringComparison.Ordinal))
        {
            return true;
        }

        var targetModule = reference["Omvila.Modules.".Length..].Split('.')[0];
        var sameModule = string.Equals(targetModule, ownModule, StringComparison.Ordinal);
        var targetIsContracts = reference.EndsWith(".Contracts", StringComparison.Ordinal);

        // Başka modül: yalnız Contracts. Aynı modül: modül kendi Contracts'ına başvurabilir; Contracts iç projeye başvuramaz.
        return targetIsContracts || (sameModule && !fromContracts);
    }

    private static IEnumerable<string> ModuleProjects()
    {
        var modulesDir = Path.Combine(SrcDir, "Modules");
        return Directory.Exists(modulesDir)
            ? Directory.EnumerateFiles(modulesDir, "*.csproj", SearchOption.AllDirectories)
            : [];
    }

    private static string ModuleName(string projectPath) =>
        Path.GetFileNameWithoutExtension(projectPath)["Omvila.Modules.".Length..].Split('.')[0];

    private static List<string> ProjectReferences(string projectPath) =>
        XDocument.Load(projectPath)
            .Descendants("ProjectReference")
            .Select(e => (string?)e.Attribute("Include") ?? string.Empty)
            .Where(include => include.Length > 0)
            // csproj yolları Windows ayırıcısıyla yazılır; Linux/macOS'ta Path bunu ayırıcı saymaz.
            .Select(include => include.Replace('\\', '/'))
            .ToList();

    private static string ApiRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Omvila.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("api/ kökü bulunamadı.");
    }
}
