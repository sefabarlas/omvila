using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Hizmet (alan-modeli.md § Service). Aşamalar ve hizmeti veren personel ayrı tablolardadır. Değerler
/// <see cref="ServiceRules"/> ile doğrulanmıştır. Kaynak türü (oda/cihaz) Dilim 1'de gizli (T-14).
/// </summary>
internal sealed class Service
{
    private Service()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CategoryId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public int DurationMinutes { get; private set; }

    public long BasePriceMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public PriceDisplay PriceDisplay { get; private set; }

    public int BufferBeforeMinutes { get; private set; }

    public int BufferAfterMinutes { get; private set; }

    public DepositMode DepositMode { get; private set; }

    public long? DepositAmountMinor { get; private set; }

    public decimal? DepositPercent { get; private set; }

    public bool? RequiresApproval { get; private set; }

    public bool IsOnlineBookable { get; private set; }

    public int? MinNoticeMinutes { get; private set; }

    public int? MaxAdvanceDays { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public uint Version { get; private set; }

    public Money BasePrice => new(BasePriceMinor, Enum.Parse<Currency>(CurrencyCode));

    public DepositRule Deposit => new(
        DepositMode, DepositAmountMinor is { } amount ? new Money(amount, Enum.Parse<Currency>(CurrencyCode)) : null, DepositPercent);

    public static Service Create(Guid id, Guid brandId, ServiceDetails details, Instant now)
    {
        var service = new Service { Id = id, BrandId = brandId, CreatedUtc = now };
        service.Update(details);
        return service;
    }

    public void Update(ServiceDetails d)
    {
        ArgumentNullException.ThrowIfNull(d);
        CategoryId = d.CategoryId;
        Name = d.Draft.Name.Trim();
        Description = string.IsNullOrWhiteSpace(d.Description) ? null : d.Description.Trim();
        DurationMinutes = d.Draft.DurationMinutes;
        BasePriceMinor = d.Draft.Price.AmountMinor;
        CurrencyCode = d.Draft.Price.Currency.ToString();
        PriceDisplay = d.Draft.PriceDisplay;
        BufferBeforeMinutes = d.Draft.BufferBeforeMinutes;
        BufferAfterMinutes = d.Draft.BufferAfterMinutes;
        DepositMode = d.Draft.Deposit.Mode;
        DepositAmountMinor = d.Draft.Deposit.Amount?.AmountMinor;
        DepositPercent = d.Draft.Deposit.Percent;
        RequiresApproval = d.RequiresApproval;
        IsOnlineBookable = d.IsOnlineBookable;
        MinNoticeMinutes = d.MinNoticeMinutes;
        MaxAdvanceDays = d.MaxAdvanceDays;
        SortOrder = d.SortOrder;
        IsActive = d.IsActive;
    }
}
