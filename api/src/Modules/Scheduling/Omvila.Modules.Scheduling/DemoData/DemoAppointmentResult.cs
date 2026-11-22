using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Yazılan demo randevusu. Tamamlanan randevuda (<see cref="DemoAppointmentOutcome.Completed"/>) tahsilatın girdisi:
/// <paramref name="CollectedMinor"/> salonda alınan ("Salonda ödenecek", tamamlamadan önce), <paramref name="RetainedDepositMinor"/>
/// salonda kalan kapora, kalemin tutarı ve paket kalemi.
/// </summary>
public sealed record DemoAppointmentResult(
    Guid Id,
    Guid ItemId,
    Instant EndUtc,
    LocalDate LocalDate,
    Currency Currency,
    long ItemAmountMinor,
    long CollectedMinor,
    long RetainedDepositMinor,
    Guid? CustomerPackageItemId);
