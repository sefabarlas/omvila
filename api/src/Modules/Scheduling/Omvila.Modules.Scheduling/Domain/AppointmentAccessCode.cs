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
    public static AppointmentAccessCode IssueForManage(Guid id, Guid brandId, Guid appointmentId, Instant appointmentEndUtc, Instant now, string? code = null) =>
        Issue(AccessCodePurpose.Manage, id, brandId, appointmentId, appointmentEndUtc, now, code);

    /// <summary>
    /// Değerlendirme bağlantısı (/d/; bildirimler.md review.invite). Davet tamamlandıktan sonra gittiğinden geçerlilik bitiş geçmişse
    /// şimdiden sayılır (docs/acik-sorular.md T-89).
    /// </summary>
    public static AppointmentAccessCode IssueForReview(Guid id, Guid brandId, Guid appointmentId, Instant appointmentEndUtc, Instant now, string? code = null) =>
        Issue(AccessCodePurpose.Review, id, brandId, appointmentId, Instant.Max(appointmentEndUtc, now), now, code);

    private static AppointmentAccessCode Issue(
        AccessCodePurpose purpose, Guid id, Guid brandId, Guid appointmentId, Instant validFromUtc, Instant now, string? code)
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
            Purpose = purpose,
            ExpiresUtc = ExpiryFor(validFromUtc),
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
