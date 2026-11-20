using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// 401 ve 403 de sözleşmedeki problem+json ile döner. Süresi dolmuş jeton 401 token_expired
/// (istemci sessizce yeniler), diğerleri 401 unauthorized; izin yok 403 forbidden (api-kurallari.md § Hatalar).
/// </summary>
internal static class JwtProblemEvents
{
    public static JwtBearerEvents Create() => new()
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            var code = context.AuthenticateFailure is SecurityTokenExpiredException
                ? ErrorCodes.TokenExpired
                : ErrorCodes.Unauthorized;
            await WriteAsync(context.HttpContext, StatusCodes.Status401Unauthorized, code);
        },
        OnForbidden = context => WriteAsync(context.HttpContext, StatusCodes.Status403Forbidden, ErrorCodes.Forbidden),
    };

    private static async Task WriteAsync(HttpContext http, int status, string code)
    {
        http.Response.StatusCode = status;
        await http.RequestServices.GetRequiredService<IProblemDetailsService>().WriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = new ProblemDetails { Status = status, Extensions = { ["code"] = code } },
        });
    }
}
