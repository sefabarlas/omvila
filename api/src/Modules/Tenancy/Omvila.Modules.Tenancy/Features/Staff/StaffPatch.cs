using System.Text.Json;
using System.Text.RegularExpressions;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Brand;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffUpdate: gelmeyen alan değişmez, null temizler (ad, renk ve oranlar boşaltılamaz).</summary>
internal sealed partial record StaffPatch(StaffProfile Profile, StaffRole? Role, IReadOnlyList<Guid>? BranchIds)
{
    public static StaffPatch Read(JsonBody input, Domain.Staff staff)
    {
        var errors = input.Errors;
        var fullName = input.String("fullName", staff.FullName);
        if (input.Has("fullName") && fullName is not { Length: >= 2 and <= 120 })
        {
            errors["fullName"] = [fullName is null ? "required" : "out_of_range"];
        }

        var color = input.String("calendarColor", staff.CalendarColor);
        if (color is null || !HexColor().IsMatch(color))
        {
            errors["calendarColor"] = ["invalid_format"];
        }

        var profile = new StaffProfile(
            fullName ?? staff.FullName,
            Max(input, "displayName", input.String("displayName", staff.DisplayName), 120),
            Max(input, "title", input.String("title", staff.Title), 60),
            Max(input, "bio", input.String("bio", staff.Bio), 1000),
            color ?? staff.CalendarColor,
            Percent(input, "serviceCommissionPercent", staff.ServiceCommissionPercent),
            Percent(input, "saleCommissionPercent", staff.SaleCommissionPercent),
            input.Bool("isBookableOnline", staff.IsBookableOnline),
            input.Bool("isActive", staff.IsActive));

        StaffRole? role = null;
        if (input.Has("role"))
        {
            var text = input.String("role", null);
            if (text is not null && Enum.GetNames<StaffRole>().Contains(text, StringComparer.Ordinal))
            {
                role = Enum.Parse<StaffRole>(text);
            }
            else
            {
                errors["role"] = ["invalid_format"];
            }
        }

        return new StaffPatch(profile, role, ReadBranchIds(input));
    }

    private static List<Guid>? ReadBranchIds(JsonBody input)
    {
        if (!input.Has("branchIds"))
        {
            return null;
        }

        var raw = input.Raw("branchIds");
        var ids = new List<Guid>();
        if (raw.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in raw.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String && item.TryGetGuid(out var id))
                {
                    ids.Add(id);
                }
                else
                {
                    input.Errors["branchIds"] = ["invalid_format"];
                    return null;
                }
            }
        }

        if (ids.Count == 0)
        {
            input.Errors["branchIds"] = ["required"];
            return null;
        }

        return [.. ids.Distinct()];
    }

    private static decimal Percent(JsonBody input, string field, decimal current)
    {
        var value = input.Decimal(field, current);
        if (value is null or < 0 or > 100)
        {
            input.Errors[field] = ["out_of_range"];
            return current;
        }

        return value.Value;
    }

    private static string? Max(JsonBody input, string field, string? value, int max)
    {
        if (value?.Length > max)
        {
            input.Errors[field] = ["too_long"];
        }

        return value;
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex HexColor();
}
