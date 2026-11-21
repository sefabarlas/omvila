using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Host.Seeding;

/// <summary>
/// <c>--role=seed [--reset]</c> (acik-sorular.md T-74): demo verisini tek transaction'da yazar ve çıkar. Yalnız Development,
/// Staging ve Testing ortamlarında çalışır; Production'da veritabanına dokunmadan 1 ile çıkar. Testing dışında
/// <c>Tenancy:IbanEncryptionKey</c> zorunludur (T-85). Hata olursa her şey geri alınır.
/// </summary>
internal static class DemoSeedCommand
{
    private const string IbanKeySetting = "Tenancy:IbanEncryptionKey";

    /// <returns>Çıkış kodu: 0 başarı, 1 izin verilmeyen ortam veya eksik IBAN şifreleme anahtarı.</returns>
    public static async Task<int> RunAsync(IServiceProvider services, IHostEnvironment environment, bool reset, TextWriter output, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(output);
        if (!environment.AllowsDemoData())
        {
            await output.WriteLineAsync($"Demo verisi {environment.EnvironmentName} ortamında yazılmaz (yalnız Development, Staging, Testing).");
            return 1;
        }

        if (IbanKeyError(environment, services.GetRequiredService<IConfiguration>()) is { } keyError)
        {
            await output.WriteLineAsync(keyError);
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

    /// <summary>
    /// Demo markasının kapora hesabı IBAN'ı şifreli yazılır; API aynı anahtarla okumalıdır (geçici anahtar süreç başınadır).
    /// Anahtar gerekiyor ve tanımlı değilse hata metni, yoksa null.
    /// </summary>
    internal static string? IbanKeyError(IHostEnvironment environment, IConfiguration configuration) =>
        environment.DemoDataRequiresIbanKey() && string.IsNullOrWhiteSpace(configuration[IbanKeySetting])
            ? $"{IbanKeySetting} tanımlı değil: demo verisi kapora IBAN'ını şifreler, API de aynı anahtarla çalışmalı. "
              + "Seed ve API için aynı değeri verin: export Tenancy__IbanEncryptionKey=\"$(openssl rand -base64 32)\""
            : null;
}
