namespace Omvila.Modules.Catalog.Domain;

/// <summary>Aşama satırı (alan-modeli.md § ServiceStage: ServiceId, SeqNo, Name, DurationMinutes, StaffBusy).</summary>
internal sealed class ServiceStageRow
{
    private ServiceStageRow()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid ServiceId { get; private set; }

    public int SeqNo { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int DurationMinutes { get; private set; }

    public bool StaffBusy { get; private set; }

    public ServiceStage ToStage() => new(Name, DurationMinutes, StaffBusy);

    public static ServiceStageRow Create(Guid id, Guid brandId, Guid serviceId, int seqNo, ServiceStage stage) => new()
    {
        Id = id,
        BrandId = brandId,
        ServiceId = serviceId,
        SeqNo = seqNo,
        Name = stage.Name.Trim(),
        DurationMinutes = stage.DurationMinutes,
        StaffBusy = stage.StaffBusy,
    };
}
