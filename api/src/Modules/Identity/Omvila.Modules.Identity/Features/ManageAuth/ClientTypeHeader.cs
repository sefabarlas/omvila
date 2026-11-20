using Microsoft.AspNetCore.Http;
using Omvila.Modules.Identity.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>X-Client-Type: web ise yenileme jetonu HttpOnly çerezde, mobile ise gövdede (sözleşme ClientType).</summary>
internal static class ClientTypeHeader
{
    public const string Name = "X-Client-Type";

    public static Result<RefreshClientType> Read(HttpRequest request) => request.Headers[Name].ToString() switch
    {
        "web" => RefreshClientType.Web,
        "mobile" => RefreshClientType.Mobile,
        "" => Error.Validation(Name, "required"),
        _ => Error.Validation(Name, "invalid_format"),
    };
}
