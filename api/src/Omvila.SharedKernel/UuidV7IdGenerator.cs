using System.Buffers.Binary;
using NodaTime;

namespace Omvila.SharedKernel;

/// <summary>
/// Zaman sıralı UUID v7. Zaman damgası enjekte edilen saatten alınır (testte sabit). Süreç içinde kesin artandır (RFC 9562 § 6.2
/// yöntem 2): aynı milisaniyede (veya saat geri gittiğinde) bir öncekinin 74 bitlik rastgele kısmı bir artırılır. Böylece
/// "son kayıt" seçen sorgular (ORDER BY an, id) aynı an damgalı satırlarda üretim sırasını bulur (T-113).
/// </summary>
public sealed class UuidV7IdGenerator(IClock clock) : IIdGenerator
{
    private const int RandomBits = 74;
    private static readonly UInt128 RandomMask = (UInt128.One << RandomBits) - 1;
    private readonly Lock _gate = new();
    private UInt128 _lastTimestamp;
    private UInt128 _lastRandom;
    private bool _hasLast;

    public Guid New()
    {
#pragma warning disable RS0030 // Tek üretim noktası burası (BannedSymbols.txt).
        var fresh = Guid.CreateVersion7(clock.GetCurrentInstant().ToDateTimeOffset());
#pragma warning restore RS0030
        Span<byte> bytes = stackalloc byte[16];
        fresh.TryWriteBytes(bytes, bigEndian: true, out _);
        var value = BinaryPrimitives.ReadUInt128BigEndian(bytes);
        var timestamp = value >> 80;
        var random = (((value >> 64) & 0xFFF) << 62) | (value & ((UInt128.One << 62) - 1));

        lock (_gate)
        {
            if (_hasLast && timestamp <= _lastTimestamp)
            {
                timestamp = _lastTimestamp;
                random = (_lastRandom + 1) & RandomMask;
                if (random == UInt128.Zero)
                {
                    timestamp++;
                }
            }

            _lastTimestamp = timestamp;
            _lastRandom = random;
            _hasLast = true;
        }

        var composed = (timestamp << 80) | ((UInt128)0x7 << 76) | ((random >> 62) << 64) | ((UInt128)0b10 << 62)
            | (random & ((UInt128.One << 62) - 1));
        BinaryPrimitives.WriteUInt128BigEndian(bytes, composed);
        return new Guid(bytes, bigEndian: true);
    }
}
