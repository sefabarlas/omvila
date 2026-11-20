using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Müşteri ekleme kuralları (sözleşme CustomerCreate; alan-modeli.md § Customer, BrandCustomer). Doğum tarihi yerine
/// yalnız doğum yılı tutulur (ürün kuralı 4); üst sınır içinde bulunulan yıldır.
/// </summary>
internal static class CustomerRules
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int NotesMaxLength = 2000;
    public const int BirthYearMin = 1900;

    public static Result<NewCustomerInput> ValidateNew(string? fullName, string? phone, int? birthYear, string? notes, int currentYear)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var name = fullName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["fullName"] = ["required"];
        }
        else if (name.Length < FullNameMinLength)
        {
            errors["fullName"] = ["too_short"];
        }
        else if (name.Length > FullNameMaxLength)
        {
            errors["fullName"] = ["too_long"];
        }

        PhoneNumber? parsed = null;
        if (string.IsNullOrWhiteSpace(phone))
        {
            errors["phone"] = ["required"];
        }
        else if (!PhoneNumber.TryParse(phone, out parsed))
        {
            errors["phone"] = ["invalid_format"];
        }

        if (birthYear is { } year && (year < BirthYearMin || year > currentYear))
        {
            errors["birthYear"] = ["out_of_range"];
        }

        var note = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (note is { Length: > NotesMaxLength })
        {
            errors["notes"] = ["too_long"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : new NewCustomerInput(parsed!, name!, birthYear, note);
    }
}
