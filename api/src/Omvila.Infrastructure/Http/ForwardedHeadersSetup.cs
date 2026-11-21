using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// Ters vekil başlıkları (T-76): X-Forwarded-For ve X-Forwarded-Proto yalnız <see cref="TrustedProxyOptions"/>'taki
/// vekil veya ağdan gelen bağlantıda uygulanır. ASP.NET'in varsayılan güvenilen geri döngü (loopback) listesi temizlenir:
/// güven yalnız yapılandırmadan gelir. Böylece IP başına OTP sınırı (OtpRateLimiting) gerçek istemci IP'sini görür,
/// güvenilmeyen istemcinin uydurduğu başlık yok sayılır.
/// </summary>
public static class ForwardedHeadersSetup
{
    public static IServiceCollection AddTrustedForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TrustedProxyOptions>()
            .Bind(configuration.GetSection(TrustedProxyOptions.Section))
            .Validate(
                o => o.InvalidEntries().Count == 0,
                $"{TrustedProxyOptions.Section}: geçersiz IP adresi veya CIDR ağı (ör. KnownProxies__0=10.0.0.5, KnownNetworks__0=172.18.0.0/16).")
            .ValidateOnStart();
        services.AddOptions<ForwardedHeadersOptions>()
            .Configure<IOptions<TrustedProxyOptions>>((options, trusted) =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // Tek vekil (Caddy): en sağdaki tek değer alınır; zincirin geri kalanı istemcinin denetimindedir.
                options.ForwardLimit = 1;
                options.KnownProxies.Clear();
                options.KnownIPNetworks.Clear();
                foreach (var proxy in trusted.Value.KnownProxies)
                {
                    options.KnownProxies.Add(IPAddress.Parse(proxy));
                }

                foreach (var network in trusted.Value.KnownNetworks)
                {
                    options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
                }
            });
        return services;
    }

    /// <summary>Hattın başında çağrılır; güvenilen vekil yapılandırılmamışsa ara katman eklenmez (başlıklar işlenmez).</summary>
    public static IApplicationBuilder UseTrustedForwardedHeaders(this IApplicationBuilder app)
    {
        var trusted = app.ApplicationServices.GetRequiredService<IOptions<TrustedProxyOptions>>().Value;
        return trusted.IsEnabled ? app.UseForwardedHeaders() : app;
    }
}
