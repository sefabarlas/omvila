using System.Reflection;
using System.Text.RegularExpressions;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Tests;

// Kural: roller-yetkiler.md § Uygulama kuralları 5 — izin kodları ve matris belgeyle birebir eşleşir.
public sealed partial class RolePermissionsDocumentTests
{
    [Fact]
    public void Matrix_MatchesDocumentTable()
    {
        var expected = ParseDocument();
        expected.Count.ShouldBeGreaterThan(30);

        foreach (var (permission, owner, manager, staff) in expected)
        {
            Scope(Roles.Owner, permission).ShouldBe(owner, $"Owner / {permission}");
            Scope(Roles.BranchManager, permission).ShouldBe(manager, $"BranchManager / {permission}");
            Scope(Roles.Staff, permission).ShouldBe(staff, $"Staff / {permission}");
        }

        RolePermissions.AllPermissions.Order(StringComparer.Ordinal)
            .ShouldBe(expected.Select(e => e.Permission).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void PermissionsConstants_AreExactlyTheDocumentCodes()
    {
        var constants = typeof(Permissions).GetNestedTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Select(f => (string)f.GetRawConstantValue()!)
            .Order(StringComparer.Ordinal);

        constants.ShouldBe(ParseDocument().Select(e => e.Permission).Order(StringComparer.Ordinal));
    }

    private static PermissionScope? Scope(string role, string permission) =>
        new RolePermissionResolver().Resolve(role, permission);

    private static List<(string Permission, PermissionScope? Owner, PermissionScope? Manager, PermissionScope? Staff)> ParseDocument()
    {
        var text = File.ReadAllText(Path.Combine(RepoRoot(), "docs", "alan", "roller-yetkiler.md"));
        var table = text.Split("## İzin kataloğu ve matris", 2)[1].Split("### Menü görünürlüğü", 2)[0];
        var rows = new List<(string, PermissionScope?, PermissionScope?, PermissionScope?)>();
        foreach (Match row in MatrixRow().Matches(table))
        {
            var codes = CodeCell().Matches(row.Groups["codes"].Value).Select(m => m.Groups["code"].Value);
            foreach (var code in codes)
            {
                rows.Add((code, Cell(row.Groups["owner"].Value, code), Cell(row.Groups["manager"].Value, code), Cell(row.Groups["staff"].Value, code)));
            }
        }

        return rows;
    }

    // ✔ tam, Ş kendi şubeleri, K yalnız kendi kayıtları, — yok. Dipnotlar belgedeki açıklamaya göre:
    // "K*" v1'de kapalı (S-07); "—**" yok; "✔***" (yöneticinin hizmet düzenlemesi) yalnız kendi şubesi.
    private static PermissionScope? Cell(string raw, string code)
    {
        var cell = raw.Trim();
        if (cell.StartsWith("K*", StringComparison.Ordinal) || cell.StartsWith('—'))
        {
            return null;
        }

        if (cell.StartsWith("✔***", StringComparison.Ordinal))
        {
            return PermissionScope.Branch;
        }

        return cell[0] switch
        {
            '✔' => PermissionScope.All,
            'Ş' => PermissionScope.Branch,
            'K' => PermissionScope.Own,
            _ => throw new InvalidOperationException($"Tanınmayan hücre '{cell}' ({code})"),
        };
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "docs", "alan")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Depo kökü bulunamadı.");
    }

    [GeneratedRegex(@"^\| (?<codes>`[^|]+) \| [^|]+ \| (?<owner>[^|]+) \| (?<manager>[^|]+) \| (?<staff>[^|]+) \|$", RegexOptions.Multiline | RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex MatrixRow();

    [GeneratedRegex("`(?<code>[a-z.-]+)`", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex CodeCell();
}
