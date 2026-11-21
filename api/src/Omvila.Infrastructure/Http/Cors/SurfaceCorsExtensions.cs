using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Http.Cors;

public static class SurfaceCorsExtensions
{
    // İstemcilerin gönderdiği başlıklar (sözleşme parametreleri + web istemcileri). Basit başlıklar (Accept, Accept-Language)
    // tarayıcıda ön uçuşsuz geçer; açıkça yazılması zararsızdır.
    internal static readonly string[] AllowedHeaders =
    [
        "Authorization", "Content-Type", "Accept", "Accept-Language",
        "If-Match", "Idempotency-Key", "X-Client-Type", "X-Client", "X-Requested-With",
    ];

    internal static readonly string[] AllowedMethods = ["GET", "POST", "PUT", "PATCH", "DELETE"];

    // Tarayıcının okuyabileceği yanıt başlıkları: ETag (If-Match için, T-59), Location (201/301), Idempotent-Replayed, Retry-After (429).
    internal static readonly string[] ExposedHeaders = ["ETag", "Location", "Idempotent-Replayed", "Retry-After"];

    /// <summary>Yüzey başına adlandırılmış CORS politikaları; kökenler "Cors:Surfaces:{Yüzey}:Origins"tan, başlangıçta denetlenir.</summary>
    internal static IServiceCollection AddSurfaceCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SurfaceCorsOptions>()
            .Bind(configuration.GetSection(SurfaceCorsOptions.Section))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<SurfaceCorsOptions>, SurfaceCorsOptionsValidator>();
        services.AddCors();
        services.AddOptions<CorsOptions>().Configure<IOptions<SurfaceCorsOptions>>((cors, surfaces) =>
        {
            foreach (var surface in ApiSurface.All)
            {
                cors.AddPolicy(surface.PolicyName, policy => Configure(policy, surface, surfaces.Value));
            }
        });
        return services;
    }

    private static void Configure(CorsPolicyBuilder policy, ApiSurface surface, SurfaceCorsOptions options)
    {
        // Köken listesi boşsa politika hiçbir kökene izin vermez (Production'da ortamdan verilmezse çapraz köken kapalı).
        policy.WithOrigins([.. options.OriginsFor(surface)])
            .WithMethods(AllowedMethods)
            .WithHeaders(AllowedHeaders)
            .WithExposedHeaders(ExposedHeaders)
            .SetPreflightMaxAge(TimeSpan.FromSeconds(options.PreflightMaxAgeSeconds));
        if (surface.AllowCredentials)
        {
            // Joker köken yasak (doğrulayıcı); kimlik bilgisiyle yalnız listelenen kökenler.
            policy.AllowCredentials();
        }
    }

    /// <summary>Grubu ön ekinin yüzey politikasına bağlar; yüzey dışı ön ek (test grubu, sağlık) CORS almaz.</summary>
    internal static RouteGroupBuilder RequireSurfaceCors(this RouteGroupBuilder group, string prefix)
    {
        if (ApiSurface.FromPrefix(prefix) is { } surface)
        {
            group.RequireCors(surface.PolicyName);
        }

        return group;
    }

    /// <summary>
    /// CORS ara katmanı: kimlik doğrulama, hız sınırı, idempotency ve veritabanı oturumundan önce çağrılır;
    /// ön uçuş (OPTIONS) isteği burada yanıtlanır ve jeton istemez.
    /// </summary>
    public static IApplicationBuilder UseSurfaceCors(this IApplicationBuilder app) => app.UseCors();
}
