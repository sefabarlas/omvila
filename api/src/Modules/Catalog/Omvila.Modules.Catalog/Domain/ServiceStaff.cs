namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Hizmeti veren personel (alan-modeli.md § ServiceStaff). Personele özel süre ve ek ücret personel uçlarında
/// ("Keratin 75 dk (özel)") düzenlenir; hizmet kaydı yalnız kimin verdiğini belirler.
/// </summary>
internal sealed class ServiceStaff
{
    private ServiceStaff()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid ServiceId { get; private set; }

    public Guid StaffId { get; private set; }

    public int? DurationOverrideMinutes { get; private set; }

    public long SurchargeMinor { get; private set; }

    public decimal? CommissionPercentOverride { get; private set; }

    public bool IsActive { get; private set; }

    public void SetOverrides(int? durationOverrideMinutes, long surchargeMinor, decimal? commissionPercentOverride)
    {
        DurationOverrideMinutes = durationOverrideMinutes;
        SurchargeMinor = surchargeMinor;
        CommissionPercentOverride = commissionPercentOverride;
        IsActive = true;
    }

    public static ServiceStaff Create(Guid id, Guid brandId, Guid serviceId, Guid staffId) => new()
    {
        Id = id,
        BrandId = brandId,
        ServiceId = serviceId,
        StaffId = staffId,
        IsActive = true,
    };
}
