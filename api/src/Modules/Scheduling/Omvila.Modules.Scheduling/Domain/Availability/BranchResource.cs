namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Şubenin kaynağı (alan-modeli.md § Resource). Tek kullanımlık türde (<see cref="IsExclusive"/>) kapasite 1 sayılır.</summary>
internal sealed record BranchResource(Guid Id, Guid ResourceTypeId, int Capacity, bool IsExclusive)
{
    public int EffectiveCapacity => IsExclusive ? 1 : Capacity;
}
