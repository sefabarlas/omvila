using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Müşterinin satın aldığı paket (alan-modeli.md § CustomerPackage; durum-makinesi.md § 5). Satışta tanımın adı, fiyatı, içeriği
/// ve şube kuralı kopyalanır; son kullanma günü satış günü + geçerlilik (şubenin yerel takvimi). Durum: Active → Expired (günlük
/// iş) | Refunded ("İade et"). Kalan seans defterden hesaplanır.
/// </summary>
internal sealed class CustomerPackage
{
    /// <summary>Sözleşme PackageRefundRequest.note maxLength.</summary>
    public const int RefundNoteMaxLength = 300;

    private readonly List<CustomerPackageItem> _items = [];

    private CustomerPackage()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    /// <summary>Satıldığı şube.</summary>
    public Guid BranchId { get; private set; }

    public Guid BrandCustomerId { get; private set; }

    public Guid PackageId { get; private set; }

    public string PackageNameSnapshot { get; private set; } = string.Empty;

    public long PricePaidMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>Satışta kopyalanan "Bütün şubelerde kullanılabilir".</summary>
    public bool IsAvailableAllBranches { get; private set; }

    public Instant PurchasedUtc { get; private set; }

    /// <summary>Son kullanım günü (şubenin yerel takvimi); bu gün dahil kullanılabilir.</summary>
    public LocalDate ExpiresOn { get; private set; }

    public Guid? SoldByStaffId { get; private set; }

    public Guid? SoldByUserId { get; private set; }

    public CustomerPackageStatus Status { get; private set; }

    public Guid? TransferredToBrandCustomerId { get; private set; }

    public long? RefundedAmountMinor { get; private set; }

    public Instant? RefundedUtc { get; private set; }

    /// <summary>Salon içi iade notu. Günlüğe yazılmaz.</summary>
    public string? RefundNote { get; private set; }

    public Guid? RefundedByUserId { get; private set; }

    public Instant? ExpiredUtc { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyList<CustomerPackageItem> Items => _items;

    public int TotalSessions => _items.Sum(i => i.TotalSessions);

    public Money PricePaid => new(PricePaidMinor, Enum.Parse<Currency>(CurrencyCode));

    /// <summary>
    /// Paket fiyatının seans payı (W2): ödenen ÷ toplam seans, kuruşa yarım yukarı. Paketten düşülen kalemin hizmet primi tabanıdır.
    /// </summary>
    // Kural: alan-modeli.md § CommissionEntry ("paketten düşülen kalemde taban, paket fiyatının seans başına payıdır"); W2
    public long SessionShareMinor => TotalSessions == 0
        ? 0
        : (long)Math.Round((decimal)PricePaidMinor / TotalSessions, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Satış: tanım satışta olmalı (değilse 409 invalid_state_transition). Fiyat tanımın fiyatıdır (indirim yok), son kullanım
    /// günü <paramref name="saleDate"/> + geçerlilik.
    /// </summary>
    // Kural: sözleşme manageSellPackage; durum-makinesi.md § 5 (— → Active)
    public static Result<CustomerPackage> Sell(
        IIdGenerator ids,
        Package package,
        Guid branchId,
        Guid brandCustomerId,
        Guid soldByStaffId,
        LocalDate saleDate,
        Guid? userId,
        Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(package);
        var allowed = package.CanSell();
        if (!allowed.IsSuccess)
        {
            return allowed.Error;
        }

        var customerPackage = new CustomerPackage
        {
            Id = ids.New(),
            BrandId = package.BrandId,
            BranchId = branchId,
            BrandCustomerId = brandCustomerId,
            PackageId = package.Id,
            PackageNameSnapshot = package.Name,
            PricePaidMinor = package.PriceMinor,
            CurrencyCode = package.CurrencyCode,
            IsAvailableAllBranches = package.IsAvailableAllBranches,
            PurchasedUtc = now,
            ExpiresOn = saleDate.PlusDays(package.ValidityDays),
            SoldByStaffId = soldByStaffId,
            SoldByUserId = userId,
            Status = CustomerPackageStatus.Active,
        };
        foreach (var item in package.Items.OrderBy(i => i.SeqNo))
        {
            customerPackage._items.Add(CustomerPackageItem.Create(ids.New(), package.BrandId, customerPackage.Id, item.SeqNo, item.ServiceId, item.SessionCount));
        }

        return customerPackage;
    }

    /// <summary>
    /// Seans kullanılabilir mi: Active, <paramref name="onDate"/> son kullanım gününü geçmemiş ve şube kuralı uyuyor (bütün
    /// şubelerde geçerli değilse yalnız satıldığı şubede). Süresi geçmiş paketin geri dönen seansı da kullanılamaz.
    /// </summary>
    // Kural: alan-modeli.md § CustomerPackage ("Süresi geçmiş paketin seansı geri dönse de kullanılamaz", IsAvailableAllBranches)
    public bool IsUsable(LocalDate onDate, Guid branchId) =>
        Status == CustomerPackageStatus.Active && onDate <= ExpiresOn && (IsAvailableAllBranches || BranchId == branchId);

    /// <summary>Günlük iş: son kullanım günü <paramref name="today"/>'den önceyse Active → Expired.</summary>
    /// <returns>Durum değiştiyse <c>true</c>.</returns>
    // Kural: durum-makinesi.md § 5 (Active → Expired, ExpiresOn geçti)
    public bool Expire(LocalDate today, Instant now)
    {
        if (Status != CustomerPackageStatus.Active || ExpiresOn >= today)
        {
            return false;
        }

        Status = CustomerPackageStatus.Expired;
        ExpiredUtc = now;
        return true;
    }

    /// <summary>
    /// "İade et" (S-10, W9): salon iadeyi kendisi yapar, tutarı elle girer. Yalnız Active veya Expired (değilse 409
    /// invalid_state_transition); tutar 0 &lt; tutar ≤ ödenen (değilse 400 <c>errors.amount = [out_of_range]</c>). Kalan seanslar
    /// kullanılamaz.
    /// </summary>
    // Kural: durum-makinesi.md § 5 (→ Refunded); sözleşme manageRefundCustomerPackage
    public Result Refund(Money amount, string? note, Guid? userId, Instant now)
    {
        if (Status is not (CustomerPackageStatus.Active or CustomerPackageStatus.Expired))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        if (!string.Equals(amount.Currency.ToString(), CurrencyCode, StringComparison.Ordinal) || amount.AmountMinor <= 0 || amount.AmountMinor > PricePaidMinor)
        {
            return Error.Validation("amount", "out_of_range");
        }

        var cleaned = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (cleaned is { Length: > RefundNoteMaxLength })
        {
            return Error.Validation("note", "too_long");
        }

        Status = CustomerPackageStatus.Refunded;
        RefundedAmountMinor = amount.AmountMinor;
        RefundedUtc = now;
        RefundNote = cleaned;
        RefundedByUserId = userId;
        return Result.Success();
    }
}
