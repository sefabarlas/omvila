using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Features.ManageAuth;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity;

/// <summary>Identity modülü girişi (ADR-0007): yüzey başına JWT şeması, izin çözümleyici, jeton üretici.</summary>
public static class IdentityModule
{
    private static readonly string[] AllAudiences = [Audiences.Manage, Audiences.Admin, Audiences.Me, Audiences.PublicBooking];

    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.Section))
            .Validate(o => o.IsValid(), "Auth ayarları eksik veya geçersiz (appsettings \"Auth\").");
        services.AddSingleton<SigningKeyStore>();
        services.AddSingleton<AccessTokenIssuer>();
        services.AddSingleton<IPermissionResolver, RolePermissionResolver>();
        services.AddModuleDbContext<IdentityDbContext>(IdentityDbContext.Schema);
        services.AddScoped<RefreshTokenService>();
        services.AddOptions<OtpOptions>()
            .Bind(configuration.GetSection(OtpOptions.Section))
            .Validate(o => o.IsValid(), "Auth:Otp ayarları eksik veya geçersiz.");
        services.AddSingleton<OtpHasher>();
        services.AddScoped<OtpService>();
        services.AddSingleton<SignupTokenService>();
        services.AddScoped<ManageSignInService>();
        services.AddOptions<LegalOptions>()
            .Bind(configuration.GetSection(LegalOptions.Section))
            .Validate(o => o.IsValid(), "Legal ayarları eksik (appsettings \"Legal\").");
        services.AddScoped<ManageSignupService>();
        services.AddOtpRateLimiting();

        // Her yüzeyin kendi şeması: bir yüzeyin jetonu (aud) diğerinde geçmez.
        var authentication = services.AddAuthentication();
        foreach (var audience in AllAudiences)
        {
            authentication.AddJwtBearer(audience, _ => { });
            services.AddOptions<JwtBearerOptions>(audience)
                .Configure<SigningKeyStore, IClock, IOptions<AuthOptions>>((options, keys, clock, auth) =>
                    Configure(options, audience, keys, clock, auth.Value));
        }

        return services;
    }

    /// <summary>Identity uçlarını eşler (manage-auth).</summary>
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        ManageAuthEndpoints.Map(app);
        return app;
    }

    private static void Configure(JwtBearerOptions options, string audience, SigningKeyStore keys, IClock clock, AuthOptions auth)
    {
        options.MapInboundClaims = false;
        options.Events = JwtProblemEvents.Create();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = auth.Issuer,
            ValidAudience = audience,
            IssuerSigningKeys = keys.Keys,
            ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
            NameClaimType = OmvilaClaims.Subject,
            RoleClaimType = OmvilaClaims.Role,
            ClockSkew = TimeSpan.Zero,
            // "Şimdi" yalnız IClock'tan (ADR-0003); testte FakeClock ile süre dolumu sınanır.
            LifetimeValidator = (notBefore, expires, _, _) =>
            {
                var now = clock.GetCurrentInstant().ToDateTimeUtc();
                if (expires is null || expires <= now)
                {
                    throw new SecurityTokenExpiredException("Erişim jetonunun süresi doldu.") { Expires = expires ?? default };
                }

                if (notBefore > now)
                {
                    throw new SecurityTokenNotYetValidException("Erişim jetonu henüz geçerli değil.") { NotBefore = notBefore.Value };
                }

                return true;
            },
        };
    }
}
