using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Bekleme tercihi biçim ve varlık denetimi (sözleşme WaitlistPreferences): 1–5 hizmet, en çok 14 gün, en çok 3 saat dilimi,
/// "Başka personel de olur" zorunlu. Hizmetler şubede çözülebilmeli, tercih edilen personel markada etkin olmalı.
/// </summary>
internal sealed class WaitlistInput(IServiceOffers offers, ITenantQueries tenants)
{
    private const int MaxTimeWindows = 3;

    public static Result<WaitlistPreferences> Parse(WaitlistPreferencesRequest? request, string prefix = "preferences")
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (request is null)
        {
            errors[prefix] = ["required"];
            return Error.Validation(errors);
        }

        if (request.ServiceIds is not { Count: > 0 } services)
        {
            errors[$"{prefix}.serviceIds"] = ["required"];
        }
        else if (services.Count > PlatformDefaults.MaxItemsPerAppointment)
        {
            errors[$"{prefix}.serviceIds"] = ["out_of_range"];
        }

        if (request.AcceptOtherStaff is null)
        {
            errors[$"{prefix}.acceptOtherStaff"] = ["required"];
        }

        if (request.Dates is null)
        {
            errors[$"{prefix}.dates"] = ["required"];
        }
        else if (request.Dates.Count > WaitlistPreferences.MaxDates)
        {
            errors[$"{prefix}.dates"] = ["out_of_range"];
        }

        var windows = new List<WaitlistTimeWindow>();
        if (request.TimeWindows is null)
        {
            errors[$"{prefix}.timeWindows"] = ["required"];
        }
        else if (request.TimeWindows.Count > MaxTimeWindows)
        {
            errors[$"{prefix}.timeWindows"] = ["out_of_range"];
        }
        else
        {
            foreach (var window in request.TimeWindows)
            {
                if (Enum.GetNames<WaitlistTimeWindow>().Contains(window, StringComparer.Ordinal))
                {
                    windows.Add(Enum.Parse<WaitlistTimeWindow>(window));
                }
                else
                {
                    errors[$"{prefix}.timeWindows"] = ["invalid_format"];
                }
            }
        }

        return errors.Count > 0
            ? Error.Validation(errors)
            : new WaitlistPreferences([.. request.ServiceIds!.Distinct()], request.PreferredStaffId, request.AcceptOtherStaff!.Value, [.. request.Dates!], windows);
    }

    /// <summary>Hizmetler şubede var mı (online kipte online açık mı), tercih edilen personel markada etkin mi.</summary>
    public async Task<Result> CheckAsync(Guid brandId, Guid branchId, WaitlistPreferences preferences, bool online, CancellationToken ct, string prefix = "preferences")
    {
        ArgumentNullException.ThrowIfNull(preferences);
        var resolved = await offers.ResolveAsync(brandId, branchId, [.. preferences.ServiceIds.Select(s => new ServiceOfferRequest(s, null))], ct);
        if (resolved.Any(r => !r.IsSuccess || (online && !r.Value.IsOnlineBookable)))
        {
            return Error.Validation($"{prefix}.serviceIds", "out_of_range");
        }

        if (preferences.PreferredStaffId is { } staffId && (await tenants.GetActiveStaffIdsAsync(brandId, [staffId], ct)).Count == 0)
        {
            return Error.Validation($"{prefix}.preferredStaffId", "out_of_range");
        }

        return Result.Success();
    }
}
