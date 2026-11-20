using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Onboarding;
using Omvila.TestKit;

namespace Omvila.Modules.Tenancy.IntegrationTests;

// Kural: dilim-01 § Alan kuralları (adımlar; hepsi tamamlanınca Brand.Status = Active), durum-makinesi.md § Brand.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class OnboardingTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid Owner = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand("TRUNCATE tenancy.brands CASCADE", connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        // Saat (PR-14) ve hizmet (Catalog) adımlarının sağlayıcıları burada tamamlanmış sayılır.
        _services = db.CreateServices(configure: services => services
            .AddTenancyModule(db.Configuration())
            .AddScoped<IOnboardingStepProvider>(_ => new DoneStep(OnboardingStep.WorkingHours))
            .AddScoped<IOnboardingStepProvider>(_ => new DoneStep(OnboardingStep.Services)));
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task AllStepsDone_ActivatesBrand_OnlyAfterLastStep()
    {
        var brandId = await InScopeAsync(null, async sp =>
            (await sp.GetRequiredService<ITenantProvisioning>().ProvisionBrandAsync("Kuaför Elif", "hair_salon", Owner, null, Ct)).BrandId);

        await InScopeAsync(brandId, sp => sp.GetRequiredService<OnboardingService>().RecordAsync(brandId, OnboardingEvent.QrDownloaded, Ct));
        (await StatusAsync(brandId)).ShouldBe("Onboarding", "Staff adımı açık");

        await InScopeAsync(brandId, sp => sp.GetRequiredService<OnboardingService>().RecordAsync(brandId, OnboardingEvent.SoloConfirmed, Ct));
        (await StatusAsync(brandId)).ShouldBe("Active");
    }

    [Fact]
    public async Task UnregisteredStep_IsNotDone()
    {
        await using var services = db.CreateServices(configure: s => s.AddTenancyModule(db.Configuration()));
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);
        var brandId = (await scope.ServiceProvider.GetRequiredService<ITenantProvisioning>()
            .ProvisionBrandAsync("Berber Ali", "barber", Owner, null, Ct)).BrandId;

        var steps = await scope.ServiceProvider.GetRequiredService<OnboardingService>().EvaluateAsync(brandId, Ct);

        steps.Single(s => s.Step == OnboardingStep.Services).Done.ShouldBeFalse();
        steps.Single(s => s.Step == OnboardingStep.WorkingHours).Done.ShouldBeFalse();
        await session.RollbackAsync(Ct);
    }

    private async Task<string> StatusAsync(Guid brandId)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand($"SELECT status FROM tenancy.brands WHERE id = '{brandId}'", connection);
        return (string)(await command.ExecuteScalarAsync(Ct))!;
    }

    private async Task InScopeAsync(Guid? brandId, Func<IServiceProvider, Task> work) =>
        await InScopeAsync(brandId, async sp =>
        {
            await work(sp);
            return 0;
        });

    private async Task<T> InScopeAsync<T>(Guid? brandId, Func<IServiceProvider, Task<T>> work)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        if (brandId is { } brand)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brand), Ct);
        }

        await session.BeginAsync(Ct);
        var result = await work(scope.ServiceProvider);
        await session.CommitAsync(Ct);
        return result;
    }

    private sealed class DoneStep(OnboardingStep step) : IOnboardingStepProvider
    {
        public OnboardingStep Step => step;

        public Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) => Task.FromResult(true);
    }
}
