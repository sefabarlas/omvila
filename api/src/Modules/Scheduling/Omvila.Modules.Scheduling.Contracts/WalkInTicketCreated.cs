using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Kapıda sıraya yeni bilet eklendi (durum-makinesi.md § 4, — → Waiting; manageCreateWalkIn). Notifications sırayı yönetenlere
/// <c>staff.walkin_queue</c> ("Sırada {n} kişi") yazar: bildirim paneli + kişisel tercihe (walkInQueue) göre işletme push'u.
/// Müşteri adı ve telefonu taşımaz.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="TicketId">Bilet.</param>
/// <param name="CreatedByUserId">Bileti ekleyen kullanıcı (bildirim almaz); yoksa null.</param>
/// <param name="WaitingCount">Bilet eklendikten sonra sırada bekleyen sayısı.</param>
/// <param name="EstimatedWaitMinutes">Yeni biletin tahmini beklemesi (dakika); müsait personel yoksa null.</param>
public sealed record WalkInTicketCreated(
    Guid BrandId,
    Guid BranchId,
    Guid TicketId,
    Guid? CreatedByUserId,
    int WaitingCount,
    int? EstimatedWaitMinutes) : IIntegrationEvent;
