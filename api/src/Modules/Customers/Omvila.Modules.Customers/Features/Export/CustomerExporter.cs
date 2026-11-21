using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Domain.Import;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Export;

/// <summary>
/// Müşterileri CSV olarak dışa aktarma (Musteriler "Dışa aktar"; sözleşme manageExportCustomers; guvenlik-kvkk.md § Haklar:
/// erişim). UTF-8 BOM'lu, noktalı virgül ayraçlı (Türkçe Excel ondalık virgül kullanır), Türkçe başlıklar. Telefon gösterim
/// biçimindedir ("0532 123 45 67"); numarası olmayan (kapıda sıra) veya çözülemeyen kayıtta boş. Birleşmiş ve
/// anonimleştirilmiş kartlar yazılmaz. Her dışa aktarma denetime yazılır (Export; filtre ve satır sayısı, kişisel veri yok).
/// </summary>
internal sealed class CustomerExporter(CustomerReader reader, IAuditLog audit)
{
    public const char Delimiter = ';';

    private static readonly string[] Headers =
        ["Ad", "Telefon", "Doğum yılı", "İlk kanal", "İlk ziyaret", "Son ziyaret", "Ziyaret", "Gelmedi", "Toplam harcama (TL)", "Etiketler", "Not"];

    private static readonly LocalDatePattern DatePattern = LocalDatePattern.CreateWithInvariantCulture("dd.MM.yyyy");

    public async Task<Result<CustomerExport>> ExportAsync(Guid brandId, string? filter, CustomerViewContext view, AuditActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(actor);
        var parsed = CustomerListFilter.All;
        if (filter is not null && (!Enum.TryParse(filter, ignoreCase: false, out parsed) || !Enum.IsDefined(parsed)))
        {
            return Error.Validation("filter", "invalid_format");
        }

        // "Paketi olan": paketler Sales modülüyle gelecek (T-46); boş dosya (yalnız başlık).
        var rows = parsed == CustomerListFilter.WithPackage
            ? []
            : await CustomerReader.Filter(reader.ListedRows(brandId), parsed, view).OrderBy(r => r.FullName).ThenBy(r => r.Id).ToListAsync(ct);

        var csv = new StringBuilder();
        csv.Append('﻿').AppendJoin(Delimiter, Headers).Append("\r\n");
        foreach (var row in rows)
        {
            csv.AppendJoin(Delimiter, Line(row, view).Select(v => CsvText.Field(v, Delimiter))).Append("\r\n");
        }

        await audit.WriteAsync(
            actor.Entry(brandId, AuditAction.Export, nameof(BrandCustomer), null, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["filter"] = parsed.ToString(),
                ["rowCount"] = rows.Count,
            }),
            ct);
        return new CustomerExport($"musteriler-{view.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}.csv", Encoding.UTF8.GetBytes(csv.ToString()));
    }

    /// <summary>"İlk kanal" metni (panel etiketleriyle aynı: Online, Panelden, Telefonla, Kapıdan; içe aktarılan: İçe aktarma).</summary>
    internal static string SourceLabel(BrandCustomerSource source) => source switch
    {
        BrandCustomerSource.Online => "Online",
        BrandCustomerSource.Panel => "Panelden",
        BrandCustomerSource.Phone => "Telefonla",
        BrandCustomerSource.WalkIn => "Kapıdan",
        BrandCustomerSource.Import => "İçe aktarma",
        _ => source.ToString(),
    };

    /// <summary>Kuruştan "1234,50" (ondalık sayı hesabı yok).</summary>
    internal static string Lira(long amountMinor) =>
        string.Create(CultureInfo.InvariantCulture, $"{amountMinor / 100},{amountMinor % 100:D2}");

    private static string?[] Line(CustomerRow row, CustomerViewContext view)
    {
        var activity = new CustomerActivity(row.VisitCount, row.NoShowCount, row.FirstVisitOn, row.LastVisitOn);
        var phone = PhoneNumber.TryParse(row.PhoneE164, out var p) ? p.Display : null;
        return
        [
            row.FullName,
            phone,
            row.BirthYear?.ToString(CultureInfo.InvariantCulture),
            SourceLabel(row.Source),
            row.FirstVisitOn is { } first ? DatePattern.Format(first) : null,
            row.LastVisitOn is { } last ? DatePattern.Format(last) : null,
            row.VisitCount.ToString(CultureInfo.InvariantCulture),
            row.NoShowCount.ToString(CultureInfo.InvariantCulture),
            Lira(row.TotalSpentMinor),
            string.Join(", ", CustomerSegments.Tags(activity, view.Today, view.Settings.RiskyCustomerNoShowThreshold)),
            row.Notes,
        ];
    }
}
