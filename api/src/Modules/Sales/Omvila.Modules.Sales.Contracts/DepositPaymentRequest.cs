using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Kapora tahsilat kaydı girdisi.</summary>
/// <param name="BrandId">Marka (kiracı bağlamıyla aynı olmalı).</param>
/// <param name="BranchId">Randevunun şubesi.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="AppointmentId">Tahsisin hedefi olan randevu.</param>
/// <param name="Amount">İstenen kapora (sıfırdan büyük).</param>
/// <param name="Reference">Havale açıklama kodu ("OMV-7K2X").</param>
/// <param name="CollectedByUserId">"Kapora geldi" diyen kullanıcı.</param>
public sealed record DepositPaymentRequest(
    Guid BrandId,
    Guid BranchId,
    Guid BrandCustomerId,
    Guid AppointmentId,
    Money Amount,
    string? Reference,
    Guid? CollectedByUserId);
