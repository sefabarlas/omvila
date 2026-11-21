using Omvila.Infrastructure.Persistence;

namespace Omvila.Host.Seeding;

/// <summary>
/// <c>--role=seed [--reset]</c> (acik-sorular.md T-74): demo verisini tek transaction'da yazar ve çıkar. Yalnız Development,
/// Staging ve Testing ortamlarında çalışır; Production'da veritabanına dokunmadan 1 ile çıkar. Hata olursa her şey geri alınır.
/// </summary>
internal static class DemoSeedCommand
{
    private static readonly string[] AllowedEnvironments = ["Development", "Staging", "Testing"];

    /// <returns>Çıkış kodu: 0 başarı, 1 izin verilmeyen ortam.</returns>
    public static async Task<int> RunAsync(IServiceProvider services, IHostEnvironment environment, bool reset, TextWriter output, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(output);
        if (!AllowedEnvironments.Contains(environment.EnvironmentName, StringComparer.Ordinal))
        {
            await output.WriteLineAsync($"Demo verisi {environment.EnvironmentName} ortamında yazılmaz (yalnız Development, Staging, Testing).");
            return 1;
        }

        DemoSeedReport report;
        await using (var scope = services.CreateAsyncScope())
        {
            var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
            await session.SetTenantAsync(TenantScope.Platform, ct);
            await session.BeginAsync(ct);
            try
            {
                report = await ActivatorUtilities.CreateInstance<DemoSeeder>(scope.ServiceProvider).RunAsync(reset, ct);
                await session.CommitAsync(ct);
            }
            catch
            {
                await session.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        await report.WriteAsync(output);
        return 0;
    }
}
