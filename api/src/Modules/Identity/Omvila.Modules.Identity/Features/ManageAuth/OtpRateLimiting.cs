using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Omvila.Modules.Identity.Features.PublicBookingAuth;
using Omvila.Modules.Identity.Security;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>
/// OTP isteği için IP başına sınır (ADR-0007; telefon başına sınır OtpService'te). Aşımda 429 rate_limited +
/// Retry-After. Ters vekil arkasında istemci IP'sini yalnız güvenilen vekilden ForwardedHeaders verir (T-76).
/// </summary>
internal static class OtpRateLimiting
{
    public static IServiceCollection AddOtpRateLimiting(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(ManageAuthEndpoints.OtpRateLimitPolicy, PerIp);
            // Rezervasyon sayfası kod istekleri ayrı sayılır (aynı sınır; güvenilir cihazla SMS'siz dönen istek de sayılır).
            options.AddPolicy(PublicOtpEndpoints.RateLimitPolicy, PerIp);
            // Müşteri uygulaması girişi ayrı sayılır (aynı sınır; T-103).
            options.AddPolicy(MeAuth.MeAuthEndpoints.OtpRateLimitPolicy, PerIp);
            // Platform paneli girişi ayrı sayılır (aynı sınır; T-119).
            options.AddPolicy(AdminAuth.AdminAuthEndpoints.OtpRateLimitPolicy, PerIp);
            options.OnRejected = async (context, ct) =>
            {
                var http = context.HttpContext;
                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    http.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                await http.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = http,
                    ProblemDetails = new ProblemDetails { Status = StatusCodes.Status429TooManyRequests, Extensions = { ["code"] = ErrorCodes.RateLimited } },
                });
            };
        });

    private static RateLimitPartition<string> PerIp(HttpContext http)
    {
        var settings = http.RequestServices.GetRequiredService<IOptions<OtpOptions>>().Value;
        return RateLimitPartition.GetFixedWindowLimiter(
            http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = settings.MaxPerIpPerHour, Window = TimeSpan.FromHours(1), QueueLimit = 0 });
    }
}
