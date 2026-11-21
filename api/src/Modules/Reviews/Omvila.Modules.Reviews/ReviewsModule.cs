using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Reviews.Features.ManageReviews;
using Omvila.Modules.Reviews.Features.PublicReviews;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.Modules.Reviews.Infrastructure.Configurations;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews;

/// <summary>
/// Reviews modülü girişi (alan-modeli.md § Modüller: Review). Dilim 5: değerlendirme daveti (<see cref="IReviewInvitations"/>,
/// <c>AppointmentCompleted</c> işleyicisi çağırır), /d/ bağlantısı uçları, panel listesi, yanıt ve müşteri kartı sekmesi,
/// rezervasyon sayfası puanı (<see cref="IReviewRatings"/>).
/// </summary>
public static class ReviewsModule
{
    public static IServiceCollection AddReviewsModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<ReviewsDbContext>(ReviewsDbContext.Schema);
        services.AddScoped<IReviewInvitations, ReviewInvitations>();
        services.AddScoped<IReviewRatings, ReviewRatings>();
        services.AddScoped<ReviewInviteViews>();
        services.AddScoped<ReviewReader>();
        // Aynı bağlantıdan eşzamanlı iki gönderim: ikincisi randevu benzersizliğine takılır.
        services.AddUniqueConstraintError(ReviewConfiguration.AppointmentIndex, ErrorCodes.AlreadyReviewed);
        // Demo verisi --reset (T-74): markanın değerlendirmeleri ve davetleri.
        services.AddBrandDataEraser<ReviewsDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapReviewsEndpoints(this IEndpointRouteBuilder app)
    {
        PublicReviewEndpoints.Map(app);
        ManageReviewEndpoints.Map(app);
        return app;
    }
}
