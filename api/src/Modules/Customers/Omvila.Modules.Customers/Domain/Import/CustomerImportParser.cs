using System.Globalization;
using System.Text;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>
/// İçe aktarma CSV'sinin satır kuralları (sözleşme managePreviewImport; T-92). Sütunlar başlıktan eşlenir (Ad, Telefon, Not,
/// Doğum yılı, E-posta; Türkçe karakter, büyük/küçük harf, boşluk ve tire yok sayılır). Ad ve Telefon sütunu zorunludur.
/// Telefon E.164'e çevrilir, yalnız +90 (T-16/T-17). Satır hataları: invalid_phone, missing_name, duplicate_in_file
/// (aynı numaranın ilk satırı geçerli kalır), too_long. Geçersiz doğum yılı satırı bozmaz, yok sayılır. Kampanya izni
/// sütunu okunmaz.
/// </summary>
internal static class CustomerImportParser
{
    public const int EmailMaxLength = 160;

    private static readonly Dictionary<string, ImportColumn> HeaderAliases = new(StringComparer.Ordinal)
    {
        ["ad"] = ImportColumn.FullName,
        ["adi"] = ImportColumn.FullName,
        ["adsoyad"] = ImportColumn.FullName,
        ["adisoyadi"] = ImportColumn.FullName,
        ["isim"] = ImportColumn.FullName,
        ["isimsoyisim"] = ImportColumn.FullName,
        ["musteri"] = ImportColumn.FullName,
        ["musteriadi"] = ImportColumn.FullName,
        ["name"] = ImportColumn.FullName,
        ["fullname"] = ImportColumn.FullName,
        ["telefon"] = ImportColumn.Phone,
        ["tel"] = ImportColumn.Phone,
        ["telefonnumarasi"] = ImportColumn.Phone,
        ["cep"] = ImportColumn.Phone,
        ["ceptelefonu"] = ImportColumn.Phone,
        ["gsm"] = ImportColumn.Phone,
        ["phone"] = ImportColumn.Phone,
        ["not"] = ImportColumn.Notes,
        ["notlar"] = ImportColumn.Notes,
        ["aciklama"] = ImportColumn.Notes,
        ["notes"] = ImportColumn.Notes,
        ["dogumyili"] = ImportColumn.BirthYear,
        ["birthyear"] = ImportColumn.BirthYear,
        ["eposta"] = ImportColumn.Email,
        ["email"] = ImportColumn.Email,
        ["mail"] = ImportColumn.Email,
    };

    public static Result<ParsedImport> Parse(string text, int maxRows, int currentYear)
    {
        ArgumentNullException.ThrowIfNull(text);
        var content = text.TrimStart('﻿');
        using var records = CsvText.Parse(content, CsvText.DetectDelimiter(content)).GetEnumerator();
        if (!records.MoveNext())
        {
            return FileError("invalid_format");
        }

        var columns = MapColumns(records.Current.Fields);
        if (!columns.ContainsKey(ImportColumn.FullName) || !columns.ContainsKey(ImportColumn.Phone))
        {
            return FileError("invalid_format");
        }

        var rows = new List<ImportRow>();
        var errors = new List<ImportRowError>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int total = 0, invalid = 0;
        while (records.MoveNext())
        {
            if (++total > maxRows)
            {
                return FileError("too_large");
            }

            var row = new RowReader(records.Current.LineNumber, records.Current.Fields, columns);
            var rowErrors = Validate(row, seen, out var phone);
            if (rowErrors.Count > 0)
            {
                invalid++;
                errors.AddRange(rowErrors);
                continue;
            }

            int? birthYear = int.TryParse(row.Value(ImportColumn.BirthYear), NumberStyles.None, CultureInfo.InvariantCulture, out var y)
                && y >= CustomerRules.BirthYearMin && y <= currentYear ? y : null;
            rows.Add(new ImportRow(row.LineNumber, phone!.E164, row.Value(ImportColumn.FullName)!, row.Value(ImportColumn.Notes), birthYear));
        }

        var mapped = Enum.GetValues<ImportColumn>().Where(columns.ContainsKey).ToList();
        return new ParsedImport(mapped, rows, errors, total, invalid);
    }

    /// <summary>Başlık eşlemesi için: Türkçe harfler sadeleşir, küçük harf, harf ve rakam dışı karakterler atılır.</summary>
    internal static string Normalize(string header)
    {
        var sb = new StringBuilder(header.Length);
        foreach (var ch in header.Trim())
        {
            var c = ch switch
            {
                'ç' or 'Ç' => 'c',
                'ğ' or 'Ğ' => 'g',
                'ı' or 'I' or 'İ' or 'i' => 'i',
                'ö' or 'Ö' => 'o',
                'ş' or 'Ş' => 's',
                'ü' or 'Ü' => 'u',
                _ => char.ToLowerInvariant(ch),
            };
            if (char.IsAsciiLetterOrDigit(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    private static Dictionary<ImportColumn, (int Index, string Header)> MapColumns(IReadOnlyList<string> header)
    {
        var columns = new Dictionary<ImportColumn, (int Index, string Header)>();
        for (var i = 0; i < header.Count; i++)
        {
            if (HeaderAliases.TryGetValue(Normalize(header[i]), out var column) && !columns.ContainsKey(column))
            {
                columns[column] = (i, header[i].Trim());
            }
        }

        return columns;
    }

    private static List<ImportRowError> Validate(RowReader row, HashSet<string> seen, out PhoneNumber? phone)
    {
        var errors = new List<ImportRowError>();
        var name = row.Value(ImportColumn.FullName);
        if (name is null || name.Length < CustomerRules.FullNameMinLength)
        {
            errors.Add(row.Error(ImportColumn.FullName, ImportErrorCode.MissingName));
        }
        else if (name.Length > CustomerRules.FullNameMaxLength)
        {
            errors.Add(row.Error(ImportColumn.FullName, ImportErrorCode.TooLong));
        }

        if (!PhoneNumber.TryParse(row.Value(ImportColumn.Phone), out phone))
        {
            errors.Add(row.Error(ImportColumn.Phone, ImportErrorCode.InvalidPhone));
        }
        else if (!seen.Add(phone.E164))
        {
            errors.Add(row.Error(ImportColumn.Phone, ImportErrorCode.DuplicateInFile));
        }

        if (row.Value(ImportColumn.Notes) is { Length: > CustomerRules.NotesMaxLength })
        {
            errors.Add(row.Error(ImportColumn.Notes, ImportErrorCode.TooLong));
        }

        if (row.Value(ImportColumn.Email) is { Length: > EmailMaxLength })
        {
            errors.Add(row.Error(ImportColumn.Email, ImportErrorCode.TooLong));
        }

        return errors;
    }

    private static Error FileError(string code) => Error.Validation("file", code);

    private readonly record struct RowReader(int LineNumber, IReadOnlyList<string> Fields, Dictionary<ImportColumn, (int Index, string Header)> Columns)
    {
        public string? Value(ImportColumn column) =>
            Columns.TryGetValue(column, out var c) && c.Index < Fields.Count && Fields[c.Index].Trim() is { Length: > 0 } v ? v : null;

        public ImportRowError Error(ImportColumn column, string code) => new(LineNumber, Columns[column].Header, code);
    }
}
