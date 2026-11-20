using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Omvila.ArchitectureTests;

// Kural: docs/mimari/mimari.md § Katman kuralları (1: Domain yalnız SharedKernel ve NodaTime'a bağlıdır)
public sealed class LayerDependencyRulesTests
{
    // Çıktı klasöründeki bütün Omvila derlemeleri (modüller glob ProjectReference ile buraya kopyalanır).
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadFilteredDirectory(AppContext.BaseDirectory, "Omvila.*.dll")
        .Build();

    private const string Frameworks = @"^(Microsoft\.AspNetCore|Microsoft\.EntityFrameworkCore|Npgsql)(\.|$)";

    [Fact]
    public void SharedKernel_DoesNotDependOnFrameworksOrOtherOmvilaProjects() =>
        Types().That().ResideInNamespaceMatching(@"^Omvila\.SharedKernel(\.|$)")
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(Frameworks)
            .AndShould().NotDependOnAnyTypesThat().ResideInNamespaceMatching(@"^Omvila\.(Host|Infrastructure|Modules)(\.|$)")
            .Check(Architecture);

    [Fact]
    public void ModuleDomain_DoesNotDependOnFrameworks() =>
        Types().That().ResideInNamespaceMatching(@"^Omvila\.Modules\.[A-Za-z]+\.Domain(\.|$)")
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(Frameworks)
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
}
