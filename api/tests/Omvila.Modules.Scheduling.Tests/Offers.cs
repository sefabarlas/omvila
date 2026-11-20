using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

/// <summary>Tasarım verisiyle teklifler (Kuaför Elif): Saç kesimi, Fön, Boya (45 + 40 bekleme + 35).</summary>
internal static class Offers
{
    public static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-00000000e11f");
    public static readonly Guid Zeynep = Guid.Parse("0193a8f0-0000-7000-8000-00000000e12a");
    public static readonly Guid HaircutId = Guid.Parse("0193a8f0-0000-7000-8000-0000000005a1");
    public static readonly Guid BlowDryId = Guid.Parse("0193a8f0-0000-7000-8000-0000000005a2");
    public static readonly Guid ColorId = Guid.Parse("0193a8f0-0000-7000-8000-0000000005a3");

    /// <summary>Sal 25 Kas 2026 14:30 İstanbul.</summary>
    public static readonly Instant Tuesday1430 = Instant.FromUtc(2026, 11, 25, 11, 30);

    public static readonly DateTimeZone Istanbul = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];

    public static ItemOffer Haircut(Guid? staffId = null, long priceMinor = 45_000) =>
        new(HaircutId, staffId ?? Elif, "Saç kesimi", Money.Try(priceMinor), Money.Zero(Currency.TRY), 0, 0, [new PlannedStage("Saç kesimi", 45, true)]);

    public static ItemOffer BlowDry(Guid? staffId = null) =>
        new(BlowDryId, staffId ?? Elif, "Fön", Money.Try(25_000), Money.Zero(Currency.TRY), 0, 0, [new PlannedStage("Fön", 30, true)]);

    public static ItemOffer Color(Guid? staffId = null) =>
        new(ColorId, staffId ?? Elif, "Boya", Money.Try(120_000), Money.Zero(Currency.TRY), 0, 0,
            [new PlannedStage("Uygulama", 45, true), new PlannedStage("Bekleme", 40, false), new PlannedStage("Yıkama ve fön", 35, true)]);
}
