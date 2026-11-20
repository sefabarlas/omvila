using NodaTime;

namespace Omvila.SharedKernel;

/// <summary>Zaman sıralı UUID v7. Zaman damgası enjekte edilen saatten alınır (testte sabit).</summary>
public sealed class UuidV7IdGenerator(IClock clock) : IIdGenerator
{
    public Guid New()
    {
#pragma warning disable RS0030 // Tek üretim noktası burası (BannedSymbols.txt).
        return Guid.CreateVersion7(clock.GetCurrentInstant().ToDateTimeOffset());
#pragma warning restore RS0030
    }
}
