using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Randevu tamamlamanın tahsilat ve prim girdisi.</summary>
/// <param name="BrandId">Marka (kiracı bağlamıyla aynı).</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="AppointmentId">Randevu (tahsis hedefi).</param>
/// <param name="Currency">Randevunun para birimi.</param>
/// <param name="Payments">Salonda alınanlar (toplamı çağıranda doğrulanmış).</param>
/// <param name="Items">Tamamlanan kalemler.</param>
/// <param name="RetainedDepositMinor">Salonda kalan kapora (alınan − iade edilen; yoksa 0).</param>
/// <param name="CompletedOn">Tamamlamanın şube yerel günü (prim dönemi).</param>
/// <param name="CollectedByUserId">Tamamlayan kullanıcı.</param>
public sealed record AppointmentCompletionRequest(
    Guid BrandId,
    Guid BranchId,
    Guid BrandCustomerId,
    Guid AppointmentId,
    Currency Currency,
    IReadOnlyList<CompletionPaymentLine> Payments,
    IReadOnlyList<CompletedItem> Items,
    long RetainedDepositMinor,
    LocalDate CompletedOn,
    Guid? CollectedByUserId);
