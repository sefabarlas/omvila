using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// CSV içe aktarma işi (alan-modeli.md § CustomerImport). Önizleme geçerli satırları (<see cref="RowsJson"/>) ve hatalı satır
/// nedenlerini (<see cref="ErrorsJson"/>; kişisel veri yok: satır, sütun, kod) saklar. Uygulanınca satırlar silinir
/// (kişisel verinin ikinci kopyası tutulmaz), sonuç sayıları kalır.
/// </summary>
internal sealed class CustomerImport
{
    public const int FileNameMaxLength = 200;

    private CustomerImport()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public int TotalRows { get; private set; }

    public int ToCreate { get; private set; }

    public int ToMerge { get; private set; }

    public int InvalidRows { get; private set; }

    /// <summary>Eşlenen sütunlar, virgülle (FullName,Phone,…).</summary>
    public string MappedColumns { get; private set; } = string.Empty;

    public string ErrorsJson { get; private set; } = "[]";

    /// <summary>Geçerli satırlar (jsonb); uygulanınca null.</summary>
    public string? RowsJson { get; private set; }

    public CustomerImportStatus Status { get; private set; }

    public bool SendInfoSms { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant ExpiresUtc { get; private set; }

    public Instant? CommittedUtc { get; private set; }

    public int Created { get; private set; }

    public int Merged { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public static CustomerImport Preview(
        Guid id, Guid brandId, string fileName, int totalRows, int toCreate, int toMerge, int invalidRows, string mappedColumns,
        string errorsJson, string rowsJson, Guid? userId, Instant now, Duration ttl)
    {
        var name = string.IsNullOrWhiteSpace(fileName) ? "musteriler.csv" : fileName.Trim();
        return new CustomerImport
        {
            Id = id,
            BrandId = brandId,
            FileName = name.Length > FileNameMaxLength ? name[..FileNameMaxLength] : name,
            TotalRows = totalRows,
            ToCreate = toCreate,
            ToMerge = toMerge,
            InvalidRows = invalidRows,
            MappedColumns = mappedColumns,
            ErrorsJson = errorsJson,
            RowsJson = rowsJson,
            Status = CustomerImportStatus.Previewed,
            CreatedUtc = now,
            ExpiresUtc = now + ttl,
            CreatedByUserId = userId,
        };
    }

    /// <summary>Uygulanabilir mi: yalnız süresi dolmamış önizleme (sınır tam eşitlikte geçmiş sayılır).</summary>
    public Result EnsureCommittable(Instant now)
    {
        if (Status != CustomerImportStatus.Previewed)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        return ExpiresUtc <= now ? Error.Conflict(ErrorCodes.HoldExpired) : Result.Success();
    }

    public void MarkCommitted(int created, int merged, bool sendInfoSms, Instant now)
    {
        Status = CustomerImportStatus.Committed;
        Created = created;
        Merged = merged;
        SendInfoSms = sendInfoSms;
        CommittedUtc = now;
        RowsJson = null;
    }
}
