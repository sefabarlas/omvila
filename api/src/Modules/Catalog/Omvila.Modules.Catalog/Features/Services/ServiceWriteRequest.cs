using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>Sözleşme ServiceWrite.</summary>
internal sealed record ServiceWriteRequest(
    Guid? CategoryId,
    string? Name,
    string? Description,
    int? DurationMinutes,
    MoneyDto? Price,
    string? PriceDisplay,
    int? BufferBeforeMinutes,
    int? BufferAfterMinutes,
    DepositDto? Deposit,
    bool? RequiresApproval,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays,
    Guid? ResourceTypeId,
    bool? IsOnlineBookable,
    bool? IsActive,
    int? SortOrder,
    IReadOnlyList<StageDto>? Stages,
    IReadOnlyList<Guid>? StaffIds)
{
    /// <summary>Biçim ve zorunluluk denetimi; değer kuralları <see cref="ServiceRules"/>'ta.</summary>
    public ServiceDetails? ToDetails(int defaultSortOrder, Dictionary<string, string[]> errors)
    {
        if (CategoryId is null)
        {
            errors["categoryId"] = ["required"];
        }

        if (Name?.Trim().Length > 120)
        {
            errors["name"] = ["too_long"];
        }

        if (Description?.Trim().Length > 1000)
        {
            errors["description"] = ["too_long"];
        }

        if (DurationMinutes is null)
        {
            errors["durationMinutes"] = ["required"];
        }

        var price = Price is null ? null : Price.ToMoney("price", errors);
        if (Price is null)
        {
            errors["price"] = ["required"];
        }

        var display = Domain.PriceDisplay.Fixed;
        if (PriceDisplay is null || !Enum.GetNames<Domain.PriceDisplay>().Contains(PriceDisplay, StringComparer.Ordinal))
        {
            errors["priceDisplay"] = [PriceDisplay is null ? "required" : "invalid_format"];
        }
        else
        {
            display = Enum.Parse<Domain.PriceDisplay>(PriceDisplay);
        }

        var deposit = Deposit is null ? null : Deposit.ToRule(errors);
        if (Deposit is null)
        {
            errors["deposit"] = ["required"];
        }

        ValidateOptional(errors);
        var stages = ParseStages(errors);

        if (errors.Count > 0)
        {
            return null;
        }

        var draft = new ServiceDraft(
            Name ?? string.Empty, DurationMinutes!.Value, price!.Value, display, BufferBeforeMinutes ?? 0, BufferAfterMinutes ?? 0, deposit!, stages);
        return new ServiceDetails(
            CategoryId!.Value, draft, Description, RequiresApproval, IsOnlineBookable!.Value, MinNoticeMinutes, MaxAdvanceDays,
            SortOrder ?? defaultSortOrder, IsActive ?? true);
    }

    private void ValidateOptional(Dictionary<string, string[]> errors)
    {
        if (IsOnlineBookable is null)
        {
            errors["isOnlineBookable"] = ["required"];
        }

        if (StaffIds is null)
        {
            errors["staffIds"] = ["required"];
        }

        if (MinNoticeMinutes is < 0)
        {
            errors["minNoticeMinutes"] = ["out_of_range"];
        }

        if (MaxAdvanceDays is < 1 or > 365)
        {
            errors["maxAdvanceDays"] = ["out_of_range"];
        }

        // T-14: oda/cihaz Dilim 1'de yok; değer gönderilirse kabul edilmez.
        if (ResourceTypeId is not null)
        {
            errors["resourceTypeId"] = ["invalid_format"];
        }
    }

    private List<ServiceStage> ParseStages(Dictionary<string, string[]> errors)
    {
        var stages = new List<ServiceStage>();
        foreach (var (stage, i) in (Stages ?? []).Select((s, i) => (s, i)))
        {
            if (stage.Name?.Trim().Length > 60)
            {
                errors[$"stages[{i}].name"] = ["too_long"];
            }

            if (stage.DurationMinutes is null || stage.StaffBusy is null)
            {
                errors[$"stages[{i}]"] = ["required"];
                continue;
            }

            stages.Add(new ServiceStage(stage.Name ?? string.Empty, stage.DurationMinutes.Value, stage.StaffBusy.Value));
        }

        return stages;
    }
}
