using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Omvila.Infrastructure.Persistence;

/// <summary>Demo verisi --reset için modül eraser kaydı (T-74).</summary>
public static class BrandDataEraserExtensions
{
    /// <summary>
    /// Modülün markaya ait tablolarını (<c>BrandId</c> özelliği olan varlıklar) silen eraser'ı kaydeder.
    /// <paramref name="brandRoot"/>: marka tablosunun kendisi (birincil anahtarı marka kimliği); en son silinir.
    /// </summary>
    public static IServiceCollection AddBrandDataEraser<TContext>(this IServiceCollection services, Type? brandRoot = null)
        where TContext : DbContext
    {
        services.AddScoped<IBrandDataEraser>(sp => new BrandDataEraser<TContext>(sp.GetRequiredService<TContext>(), brandRoot));
        return services;
    }
}
