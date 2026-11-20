using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Omvila.Infrastructure.Http;

public static class EndpointConventionExtensions
{
    /// <summary>Uç If-Match başlığı ister (PUT/PATCH ve durum eylemleri).</summary>
    public static RouteHandlerBuilder RequireIfMatch(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<IfMatchEndpointFilter>()
            .ProducesProblem(StatusCodes.Status412PreconditionFailed)
            .ProducesProblem(StatusCodes.Status428PreconditionRequired);

    /// <summary>Benzersiz indeks ihlalini (23505) iş hatası koduna eşler.</summary>
    public static IServiceCollection AddUniqueConstraintError(this IServiceCollection services, string constraintName, string code) =>
        services.AddSingleton(new UniqueConstraintError(constraintName, code));
}
