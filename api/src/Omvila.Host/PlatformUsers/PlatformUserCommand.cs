using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Host.PlatformUsers;

/// <summary>
/// <c>--role=platform-user --platform-phone=… --platform-name=… --platform-role=PlatformAdmin|PlatformSupport|PlatformFinance</c>
/// (K2, acik-sorular.md T-119): platform kullanıcısını tek transaction'da oluşturur veya rolünü ve adını günceller, sonra çıkar.
/// Platform kullanıcıları web'den kaydolmaz. Her ortamda çalışır (test sunucusunda ve canlıda ekip hesabı bu komutla açılır).
/// Numaranın marka üyeliği varsa hiçbir şey yazmaz (platform kullanıcısı ile salon üyeliği birbirini dışlar). Çıktıda telefon maskelidir.
/// </summary>
internal static class PlatformUserCommand
{
    public const string Usage =
        "Kullanım: --role=platform-user --platform-phone=05327001020 --platform-name=\"Ad Soyad\" --platform-role=PlatformAdmin|PlatformSupport|PlatformFinance";

    /// <returns>Çıkış kodu: 0 başarı, 1 eksik veya geçersiz değer ya da numaranın marka üyeliği var.</returns>
    public static async Task<int> RunAsync(IServiceProvider services, IConfiguration configuration, TextWriter output, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(output);
        var name = configuration["platform-name"];
        var role = configuration["platform-role"];
        if (!PhoneNumber.TryParse(configuration["platform-phone"], out var phone) || !phone.IsMobile
            || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
        {
            await output.WriteLineAsync("Telefon (cep), ad ve platform rolü zorunludur. " + Usage);
            return 1;
        }

        Result<ProvisionedPlatformUser> result;
        await using (var scope = services.CreateAsyncScope())
        {
            var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
            await session.SetTenantAsync(TenantScope.Platform, ct);
            await session.BeginAsync(ct);
            try
            {
                result = await scope.ServiceProvider.GetRequiredService<IPlatformUserProvisioning>().EnsureAsync(phone, name, role, ct);
                if (result.IsSuccess)
                {
                    await session.CommitAsync(ct);
                }
                else
                {
                    await session.RollbackAsync(ct);
                }
            }
            catch
            {
                await session.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        if (!result.IsSuccess)
        {
            await output.WriteLineAsync("Platform rolü geçersiz veya ad çok uzun. " + Usage);
            return 1;
        }

        await output.WriteLineAsync(result.Value.Created
            ? $"Platform kullanıcısı oluşturuldu: {phone.Masked} ({role})."
            : $"Platform kullanıcısı güncellendi: {phone.Masked} ({role}).");
        return 0;
    }
}
