using System.Text.RegularExpressions;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Personel ekleme kuralları (sözleşme StaffCreate; ADR-0007 personel daveti): ad 2–120; en az bir şube;
/// prim 0–100; davet SMS'i için cep telefonu zorunlu (giriş SMS koduyla); telefon verilirse cep telefonu olmalı.
/// Takvim rengi biçimi #RRGGBB (palet denetimi S-27: tasarım tokenı tanımlanınca).
/// </summary>
internal static partial class StaffRules
{
    public static Result<PhoneNumber?> Validate(StaffDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var name = draft.FullName?.Trim() ?? string.Empty;
        if (name.Length is < 2 or > 120)
        {
            errors["fullName"] = [name.Length == 0 ? "required" : "out_of_range"];
        }

        if (draft.BranchIds.Count == 0)
        {
            errors["branchIds"] = ["required"];
        }

        if (draft.ServiceCommissionPercent is < 0 or > 100)
        {
            errors["serviceCommissionPercent"] = ["out_of_range"];
        }

        if (draft.SaleCommissionPercent is < 0 or > 100)
        {
            errors["saleCommissionPercent"] = ["out_of_range"];
        }

        if (!HexColor().IsMatch(draft.CalendarColor ?? string.Empty))
        {
            errors["calendarColor"] = ["invalid_format"];
        }

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(draft.PhoneInput))
        {
            if (!PhoneNumber.TryParse(draft.PhoneInput, out phone) || !phone.IsMobile)
            {
                errors["phone"] = ["invalid_format"];
            }
        }
        else if (draft.SendInvite)
        {
            errors["phone"] = ["required"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : phone;
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex HexColor();
}
