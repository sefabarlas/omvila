using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>Kurulum uçları (sözleşme manage-session: manageGetOnboarding, manageRecordOnboardingEvent).</summary>
internal static class OnboardingEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/onboarding", Audiences.Manage);

        // Kurulum kartı her rolde okunabilir (izin gerektirmez); adımları değiştiren olaylar yalnız ayar yetkisiyle.
        group.MapGet(string.Empty, GetAsync).WithName("manageGetOnboarding").RequireAuthenticated(Audiences.Manage);
        group.MapPost("/events", RecordAsync).WithName("manageRecordOnboardingEvent").RequirePermission(Permissions.Settings.Manage);
    }

    private static async Task<IResult> GetAsync(ITenantContext tenant, OnboardingService onboarding, CancellationToken ct)
    {
        var steps = await onboarding.EvaluateAsync(BrandOf(tenant), ct);
        return TypedResults.Ok(new OnboardingChecklist(
            steps.Count(s => s.Done),
            steps.Count,
            [.. steps.Select(s => new OnboardingStepState(s.Step.ToString(), s.Done))]));
    }

    private static async Task<IResult> RecordAsync(OnboardingEventRequest body, ITenantContext tenant, OnboardingService onboarding, CancellationToken ct)
    {
        // Yalnız sözleşmedeki adlar (sayısal veya küçük harfli değer kabul edilmez).
        if (body.Event is null || !Enum.GetNames<OnboardingEvent>().Contains(body.Event, StringComparer.Ordinal)
            || !Enum.TryParse<OnboardingEvent>(body.Event, out var onboardingEvent))
        {
            return Error.Validation("event", body.Event is null ? "required" : "invalid_format").ToProblem();
        }

        await onboarding.RecordAsync(BrandOf(tenant), onboardingEvent, ct);
        return TypedResults.NoContent();
    }

    // manage yüzeyinde kiracı bağlamı jetondan kurulur (DbSessionMiddleware); markasız manage jetonu olmaz.
    private static Guid BrandOf(ITenantContext tenant) =>
        tenant.Scope.BrandId ?? throw new DomainException("Manage isteğinde marka bağlamı yok.");
}
