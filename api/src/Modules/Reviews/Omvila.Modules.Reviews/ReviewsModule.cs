using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Contracts;
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
        services.AddIntegrationEventHandler<AppointmentCompleted, IntegrationEventHandlers.ScheduleReviewInvite>();
        services.AddScoped<IReviewRatings, ReviewRatings>();
        // Müşteri uygulaması randevu satırlarındaki değerlendirme özeti (T-104).
        services.AddScoped<ICustomerReviewSummaries, CustomerReviewSummaries>();
        services.AddScoped<ReviewInviteViews>();
        services.AddScoped<ReviewReader>();
        // Müşteri birleştirmede değerlendirmeler kalan karta taşınır (T-92).
        services.AddScoped<Customers.Contracts.ICustomerMergeParticipant, CustomerReviewsParticipant>();
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
        Features.MeReviews.MeReviewEndpoints.Map(app);
        return app;
    }
}
