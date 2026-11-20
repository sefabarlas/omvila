using Microsoft.AspNetCore.Http;
using NodaTime;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Tests;

// Kural: ADR-0007 (HttpOnly; Secure; SameSite=Strict; Path=/api/v1/manage/auth; hatırla → kalıcı, değilse oturum çerezi; CSRF)
public sealed class RefreshTokenCookieTests
{
    private static readonly Instant Expires = Instant.FromUtc(2026, 12, 25, 9, 0);

    [Fact]
    public void Append_WithRemember_WritesPersistentStrictCookie()
    {
        var header = Append(remember: true);

        header.ShouldStartWith("omvila_rt=abc");
        header.ShouldContain("expires=", Case.Insensitive);
        header.ShouldContain("path=/api/v1/manage/auth", Case.Insensitive);
        header.ShouldContain("secure", Case.Insensitive);
        header.ShouldContain("samesite=strict", Case.Insensitive);
        header.ShouldContain("httponly", Case.Insensitive);
    }

    [Fact]
    public void Append_WithoutRemember_WritesSessionCookie() =>
        Append(remember: false).ShouldNotContain("expires=", Case.Insensitive);

    [Fact]
    public async Task XRequestedWith_MissingOnCookieRequest_Returns401()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "omvila_rt=abc";

        var result = await Invoke(context);

        (result as IStatusCodeHttpResult)?.StatusCode.ShouldBe(401);
    }

    [Fact]
    public async Task XRequestedWith_PresentOnCookieRequest_OrBodyTokenRequest_Passes()
    {
        var withHeader = new DefaultHttpContext();
        withHeader.Request.Headers.Cookie = "omvila_rt=abc";
        withHeader.Request.Headers[XRequestedWithFilter.HeaderName] = "XMLHttpRequest";

        (await Invoke(withHeader)).ShouldBe("next");
        (await Invoke(new DefaultHttpContext())).ShouldBe("next");
    }

    private static string Append(bool remember)
    {
        var context = new DefaultHttpContext();
        RefreshTokenCookie.Append(context.Response, new IssuedRefreshToken("abc", Expires, Guid.Empty, null, remember));
        return context.Response.Headers.SetCookie.ToString();
    }

    private static ValueTask<object?> Invoke(HttpContext context) =>
        new XRequestedWithFilter().InvokeAsync(EndpointFilterInvocationContext.Create(context), _ => ValueTask.FromResult<object?>("next"));
}
