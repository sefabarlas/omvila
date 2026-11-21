using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Paket tanımı (alan-modeli.md § Package, PackageItem; Paketler, PaketOlustur). Değişiklik yalnız sonraki satışları etkiler:
/// satışta ad, içerik, fiyat ve şube kuralı müşteri paketine kopyalanır. Paketler silinmez, "Satıştan kaldır" ile pasifleşir.
/// Devir (IsTransferable) v1'de yoktur (T-86 (3)).
/// </summary>
internal sealed class Package
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 80;
    public const int MaxItems = 10;
    public const int MaxSessionCount = 100;
    public const int MaxValidityDays = 1095;

    private readonly List<PackageItem> _items = [];

    private Package()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public long PriceMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>"Geçerlilik", satış gününden itibaren gün (şubenin yerel takvimi).</summary>
    public int ValidityDays { get; private set; }

    /// <summary>"Satış primi" (pakete özel); null ise satan personelin oranı.</summary>
    public decimal? SaleCommissionPercentOverride { get; private set; }

    public bool IsAvailableAllBranches { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant? UpdatedUtc { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyList<PackageItem> Items => _items;

    public Money Price => new(PriceMinor, Enum.Parse<Currency>(CurrencyCode));

    /// <summary>"Seans" sütunu: içerikteki seansların toplamı.</summary>
    public int TotalSessions => _items.Sum(i => i.SessionCount);

    // Kural: alan-modeli.md § Package
    public static Package Create(IIdGenerator ids, Guid brandId, PackageDraft draft, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        var package = new Package { Id = ids.New(), BrandId = brandId, CreatedUtc = now };
        package.Apply(ids, draft);
        return package;
    }

    /// <summary>Tanımı değiştirir; satılmış paketler etkilenmez (satışta kopyalandılar). İçerik satırları yeniden yazılır.</summary>
    /// <returns>Kaldırılan eski içerik satırları (çağıran siler).</returns>
    // Kural: sözleşme manageUpdatePackage ("Değişiklik yalnız sonraki satışları etkiler")
    public IReadOnlyList<PackageItem> Update(IIdGenerator ids, PackageDraft draft, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        var removed = _items.ToList();
        Apply(ids, draft);
        UpdatedUtc = now;
        return removed;
    }

    /// <summary>Satış koruması: "Satışta değil" paket satılamaz (409 invalid_state_transition).</summary>
    public Result CanSell() => IsActive ? Result.Success() : Error.Conflict(ErrorCodes.InvalidStateTransition);

    private void Apply(IIdGenerator ids, PackageDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var name = draft.Name.Trim();
        if (name.Length is < NameMinLength or > NameMaxLength)
        {
            throw new DomainException("Paket adı 2–80 karakter olmalı.");
        }

        if (draft.Price.AmountMinor <= 0)
        {
            throw new DomainException("Paket fiyatı sıfırdan büyük olmalı.");
        }

        if (draft.Items.Count is 0 or > MaxItems
            || draft.Items.Any(i => i.SessionCount is < 1 or > MaxSessionCount)
            || draft.Items.Select(i => i.ServiceId).Distinct().Count() != draft.Items.Count)
        {
            throw new DomainException("Paket içeriği 1–10 farklı hizmet, her biri 1–100 seans olmalı.");
        }

        if (draft.ValidityDays is < 1 or > MaxValidityDays)
        {
            throw new DomainException("Paket geçerliliği 1–1095 gün olmalı.");
        }

        if (draft.SaleCommissionPercent is < 0 or > 100)
        {
            throw new DomainException("Satış primi 0–100 olmalı.");
        }

        Name = name;
        PriceMinor = draft.Price.AmountMinor;
        CurrencyCode = draft.Price.Currency.ToString();
        ValidityDays = draft.ValidityDays;
        SaleCommissionPercentOverride = draft.SaleCommissionPercent;
        IsAvailableAllBranches = draft.IsAvailableAllBranches;
        IsActive = draft.IsActive;
        _items.Clear();
        for (var i = 0; i < draft.Items.Count; i++)
        {
            _items.Add(PackageItem.Create(ids.New(), BrandId, Id, i + 1, draft.Items[i]));
        }
    }
}
