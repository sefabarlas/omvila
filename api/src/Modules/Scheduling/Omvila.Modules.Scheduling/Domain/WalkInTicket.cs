using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kapıda sıra bileti (alan-modeli.md § WalkInTicket). Numara şube + şubenin yerel günü bazında 1'den verilir
/// (<see cref="BranchQueueCounter"/>). Müşteri kaydı bilet oluşurken değil "Başlat"ta açılır (T-86 W6); hizmet adı ve süresi
/// tahmini bekleme için katılımda kopyalanır.
/// </summary>
internal sealed class WalkInTicket
{
    private WalkInTicket()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    /// <summary>Şubenin yerel günü.</summary>
    public LocalDate QueueDate { get; private set; }

    public int QueueNumber { get; private set; }

    /// <summary>Var olan müşteri seçildiyse veya bilet randevuya çevrildiyse.</summary>
    public Guid? BrandCustomerId { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>İsteğe bağlı; "Sırası yaklaştı" SMS'i için.</summary>
    public string? PhoneE164 { get; private set; }

    public Guid? ServiceId { get; private set; }

    public string? ServiceName { get; private set; }

    /// <summary>Katılımda çözülen hizmet süresi (WalkInAverageServiceMinutes "hizmet süresinden").</summary>
    public int? ServiceMinutes { get; private set; }

    /// <summary>null = fark etmez.</summary>
    public Guid? PreferredStaffId { get; private set; }

    public WalkInTicketStatus Status { get; private set; }

    public Instant JoinedUtc { get; private set; }

    public Instant? CalledUtc { get; private set; }

    /// <summary>Served veya Left anı.</summary>
    public Instant? ClosedUtc { get; private set; }

    public Guid? AppointmentId { get; private set; }

    /// <summary>walkin.almost_turn yazıldı (bilet başına bir kez).</summary>
    public Instant? AlmostTurnNotifiedUtc { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public uint Version { get; private set; }

    public bool IsInQueue => Status is WalkInTicketStatus.Waiting or WalkInTicketStatus.Called;

    public static WalkInTicket Create(NewWalkInTicket input, Guid id, LocalDate queueDate, int queueNumber, Instant now)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (string.IsNullOrWhiteSpace(input.CustomerName))
        {
            throw new DomainException("Bilette ad zorunludur.");
        }

        if (queueNumber < 1)
        {
            throw new DomainException("Sıra numarası 1'den başlar.");
        }

        return new WalkInTicket
        {
            Id = id,
            BrandId = input.BrandId,
            BranchId = input.BranchId,
            QueueDate = queueDate,
            QueueNumber = queueNumber,
            BrandCustomerId = input.BrandCustomerId,
            CustomerName = input.CustomerName.Trim(),
            PhoneE164 = input.Phone?.E164,
            ServiceId = input.ServiceId,
            ServiceName = input.ServiceName,
            ServiceMinutes = input.ServiceMinutes,
            PreferredStaffId = input.PreferredStaffId,
            Status = WalkInTicketStatus.Waiting,
            JoinedUtc = now,
            CreatedByUserId = input.CreatedByUserId,
        };
    }

    /// <summary>"Başlat / Randevuya çevir": Waiting veya Called → Served.</summary>
    // Kural: docs/alan/durum-makinesi.md#4-kapıda-sıra-walkinticketstatus
    public Result Serve(Guid appointmentId, Guid brandCustomerId, Instant now)
    {
        if (!IsInQueue)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = WalkInTicketStatus.Served;
        AppointmentId = appointmentId;
        BrandCustomerId = brandCustomerId;
        ClosedUtc = now;
        return Result.Success();
    }

    /// <summary>"Sıradan çıkar": Waiting veya Called → Left.</summary>
    // Kural: docs/alan/durum-makinesi.md#4-kapıda-sıra-walkinticketstatus
    public Result Leave(Instant now)
    {
        if (!IsInQueue)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = WalkInTicketStatus.Left;
        ClosedUtc = now;
        return Result.Success();
    }

    /// <summary>
    /// Önünde 1 kişi kaldı: telefon verildiyse ve daha önce yazılmadıysa işaretler ve true döner (bildirimler.md
    /// walkin.almost_turn, bilet başına bir kez).
    /// </summary>
    public bool TryMarkAlmostTurn(Instant now)
    {
        if (!IsInQueue || PhoneE164 is null || AlmostTurnNotifiedUtc is not null)
        {
            return false;
        }

        AlmostTurnNotifiedUtc = now;
        return true;
    }
}
