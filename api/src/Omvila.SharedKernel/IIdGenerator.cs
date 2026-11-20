namespace Omvila.SharedKernel;

/// <summary>Kimlik üretimi. Tek uygulama: <see cref="UuidV7IdGenerator"/> (docs/mimari/mimari.md § Kesişen konular).</summary>
public interface IIdGenerator
{
    Guid New();
}
