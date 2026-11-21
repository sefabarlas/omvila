using NodaTime;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Seans ayırma talebi.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BranchId">Randevunun şubesi (paket şube kuralı).</param>
/// <param name="BrandCustomerId">Randevunun müşteri kartı (paket bu müşterinin olmalı).</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="AppointmentDate">Randevunun şube yerel günü (son kullanım gününü geçemez).</param>
/// <param name="Claims">Paketten düşülecek kalemler.</param>
public sealed record PackageSessionReservation(
    Guid BrandId,
    Guid BranchId,
    Guid BrandCustomerId,
    Guid AppointmentId,
    LocalDate AppointmentDate,
    IReadOnlyList<PackageSessionClaim> Claims);
