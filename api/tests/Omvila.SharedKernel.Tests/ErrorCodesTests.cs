using System.Text.RegularExpressions;
using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

// Kural: kodlama-dotnet.md § Hata eşleme — ErrorCodes, sözleşmedeki ErrorCode enum'uyla birebir aynıdır.
public sealed partial class ErrorCodesTests
{
    [Fact]
    public void All_MatchesContractErrorCodeEnum()
    {
        var contract = File.ReadAllText(Path.Combine(RepoRoot(), "contracts", "openapi.yaml"));
        var block = contract.Split("    ErrorCode:\n", 2)[1].Split("\n    Money:", 2)[0];
        var contractCodes = EnumItem().Matches(block).Select(m => m.Groups["code"].Value).ToHashSet(StringComparer.Ordinal);

        contractCodes.ShouldNotBeEmpty();
        ErrorCodes.All.Except(contractCodes, StringComparer.Ordinal).ShouldBeEmpty("sözleşmede olmayan kod");
        contractCodes.Except(ErrorCodes.All, StringComparer.Ordinal).ShouldBeEmpty("ErrorCodes'ta eksik kod");
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "contracts", "openapi.yaml")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Depo kökü bulunamadı.");
    }

    [GeneratedRegex(@"^        - (?<code>[a-z_]+)$", RegexOptions.Multiline | RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex EnumItem();
}
