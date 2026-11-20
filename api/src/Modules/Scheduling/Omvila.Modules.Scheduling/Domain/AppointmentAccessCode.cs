using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// SMS'teki <c>omvila.com/r/{code}</c> bağlantısı (alan-modeli.md § AppointmentAccessCode). Bağlantıyı açan kişi o
/// randevu için kimliği doğrulanmış sayılır. Randevu bitişinden
/// <see cref="PlatformDefaults.AccessCodeValidityAfterEndDays"/> gün sonra geçersizdir.
/// </summary>
internal sealed class AppointmentAccessCode
{
    private AppointmentAccessCode()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid AppointmentId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public AccessCodePurpose Purpose { get; private set; }

    public Instant ExpiresUtc { get; private set; }

    public Instant? RevokedUtc { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>Randevu yönetim bağlantısı (/r/). Kod verilmezse üretilir (testte sabit kod verilebilir).</summary>
    public static AppointmentAccessCode IssueForManage(Guid id, Guid brandId, Guid appointmentId, Instant appointmentEndUtc, Instant now, string? code = null)
    {
        code ??= AccessCodeGenerator.New();
        if (!AccessCodeGenerator.IsWellFormed(code))
        {
            throw new DomainException("Erişim kodu 10 karakter Base62 olmalı.");
        }

        return new AppointmentAccessCode
        {
            Id = id,
            BrandId = brandId,
            AppointmentId = appointmentId,
            Code = code,
            Purpose = AccessCodePurpose.Manage,
            ExpiresUtc = ExpiryFor(appointmentEndUtc),
            CreatedUtc = now,
        };
    }

    public static Instant ExpiryFor(Instant appointmentEndUtc) =>
        appointmentEndUtc + Duration.FromDays(PlatformDefaults.AccessCodeValidityAfterEndDays);

    /// <summary>Saat değişince geçerlilik yeni bitişe göre yeniden hesaplanır.</summary>
    public void FollowAppointmentEnd(Instant appointmentEndUtc) => ExpiresUtc = ExpiryFor(appointmentEndUtc);

    public void Revoke(Instant now) => RevokedUtc ??= now;

    public bool IsValidAt(Instant now) => RevokedUtc is null && now < ExpiresUtc;
}
