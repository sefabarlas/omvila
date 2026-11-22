using System.Reflection;
using System.Text.RegularExpressions;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Tests;

// Kural: roller-yetkiler.md § Platform rolleri (K3) — AdminPermission kodları ve rol matrisi belgeyle birebir; Destek kontör
// ekleyemez, plan ve durum değiştiremez (dilim-06 KK4).
public sealed partial class PlatformRolePermissionsDocumentTests
{
    [Fact]
    public void Matrix_MatchesDocumentTable()
    {
        var expected = ParseDocument();
        expected.Count.ShouldBe(10);
        var resolver = new PlatformRolePermissionResolver();

        foreach (var (permission, admin, support, finance) in expected)
        {
            resolver.HasPermission(PlatformRoles.Admin, permission).ShouldBe(admin, $"PlatformAdmin / {permission}");
            resolver.HasPermission(PlatformRoles.Support, permission).ShouldBe(support, $"PlatformSupport / {permission}");
            resolver.HasPermission(PlatformRoles.Finance, permission).ShouldBe(finance, $"PlatformFinance / {permission}");
        }

        PlatformRolePermissions.AllPermissions.Order(StringComparer.Ordinal)
            .ShouldBe(expected.Select(e => e.Permission).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void AdminPermissionConstants_AreExactlyTheDocumentCodes()
    {
        var constants = typeof(AdminPermissions).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (string)f.GetRawConstantValue()!)
            .Order(StringComparer.Ordinal);

        constants.ShouldBe(ParseDocument().Select(e => e.Permission).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void SupportRole_CannotGrantCreditsOrManageBrands_AndUnknownRoleHasNothing()
    {
        var resolver = new PlatformRolePermissionResolver();

        resolver.HasPermission(PlatformRoles.Support, AdminPermissions.CreditsGrant).ShouldBeFalse();
        resolver.HasPermission(PlatformRoles.Support, AdminPermissions.BrandsManage).ShouldBeFalse();
        resolver.HasPermission(PlatformRoles.Support, AdminPermissions.BrandsView).ShouldBeTrue();
        resolver.HasPermission(Roles.Owner, AdminPermissions.BrandsView).ShouldBeFalse("salon rolü platform izni taşımaz");
    }

    private static List<(string Permission, bool Admin, bool Support, bool Finance)> ParseDocument()
    {
        var text = File.ReadAllText(Path.Combine(RepoRoot(), "docs", "alan", "roller-yetkiler.md"));
        var table = text.Split("## Platform rolleri", 2)[1].Split("\n## ", 2)[0];
        return [.. PlatformRow().Matches(table).Select(m => (
            m.Groups["code"].Value,
            Cell(m.Groups["admin"].Value),
            Cell(m.Groups["support"].Value),
            Cell(m.Groups["finance"].Value)))];
    }

    private static bool Cell(string raw) => raw.Trim() switch
    {
        "✔" => true,
        "—" => false,
        var other => throw new InvalidOperationException($"Tanınmayan hücre '{other}'"),
    };

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "docs", "alan")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Depo kökü bulunamadı.");
    }

    [GeneratedRegex(@"^\| [^|]+ \| `(?<code>platform\.[a-z.-]+)` \| (?<admin>[^|]+) \| (?<support>[^|]+) \| (?<finance>[^|]+) \|$", RegexOptions.Multiline | RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex PlatformRow();
}
