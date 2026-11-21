namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Bir adayda kalemin personeli, başlangıca bağlanmış planı ve ayrılan kaynakları.</summary>
internal sealed record AssignedItem(Guid StaffId, ItemPlan Plan, IReadOnlyList<ResourceAllocation> Resources)
{
    public Guid ServiceId => Plan.Offer.ServiceId;
}
