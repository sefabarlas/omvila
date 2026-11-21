using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageWrite ("Paket oluştur").</summary>
internal sealed record PackageWriteRequest(
    string? Name,
    MoneyInput? Price,
    IReadOnlyList<PackageItemWriteRequest>? Items,
    int? ValidityDays,
    decimal? SaleCommissionPercent,
    bool? IsAvailableAllBranches,
    bool? IsActive)
{
    /// <summary>
    /// Biçim denetimi (400 validation_failed). Hizmetlerin markada etkin olması <paramref name="activeServiceIds"/> ile denetlenir;
    /// tekrar eden veya etkin olmayan hizmet <c>errors.items</c> (sözleşme manageCreatePackage).
    /// </summary>
    public PackageDraft? ToDraft(Currency brandCurrency, IReadOnlySet<Guid> activeServiceIds, Dictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(activeServiceIds);
        var name = Name?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["name"] = ["required"];
        }
        else if (name.Length is < Package.NameMinLength or > Package.NameMaxLength)
        {
            errors["name"] = ["out_of_range"];
        }

        var price = MoneyInput.Read(Price, "price", brandCurrency, errors);
        var items = ReadItems(activeServiceIds, errors);
        if (ValidityDays is null)
        {
            errors["validityDays"] = ["required"];
        }
        else if (ValidityDays is < 1 or > Package.MaxValidityDays)
        {
            errors["validityDays"] = ["out_of_range"];
        }

        if (SaleCommissionPercent is < 0 or > 100)
        {
            errors["saleCommissionPercent"] = ["out_of_range"];
        }

        if (IsAvailableAllBranches is null)
        {
            errors["isAvailableAllBranches"] = ["required"];
        }

        if (IsActive is null)
        {
            errors["isActive"] = ["required"];
        }

        return errors.Count > 0
            ? null
            : new PackageDraft(name!, price!.Value, items, ValidityDays!.Value, SaleCommissionPercent, IsAvailableAllBranches!.Value, IsActive!.Value);
    }

    private List<PackageDraftItem> ReadItems(IReadOnlySet<Guid> activeServiceIds, Dictionary<string, string[]> errors)
    {
        var items = new List<PackageDraftItem>();
        if (Items is null || Items.Count == 0)
        {
            errors["items"] = ["required"];
            return items;
        }

        if (Items.Count > Package.MaxItems)
        {
            errors["items"] = ["out_of_range"];
            return items;
        }

        var seen = new HashSet<Guid>();
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (item?.ServiceId is not { } serviceId)
            {
                errors[$"items[{i}].serviceId"] = ["required"];
                continue;
            }

            if (!activeServiceIds.Contains(serviceId) || !seen.Add(serviceId))
            {
                errors["items"] = ["invalid_format"];
                continue;
            }

            if (item.SessionCount is not { } count || count is < 1 or > Package.MaxSessionCount)
            {
                errors[$"items[{i}].sessionCount"] = [item.SessionCount is null ? "required" : "out_of_range"];
                continue;
            }

            items.Add(new PackageDraftItem(serviceId, count));
        }

        return items;
    }

    /// <summary>İstekteki hizmet kimlikleri (etkinlik sorgusu için).</summary>
    public IReadOnlyList<Guid> ServiceIds => [.. (Items ?? []).Where(i => i?.ServiceId is not null).Select(i => i.ServiceId!.Value).Distinct()];
}
