using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Belirli bir başlangıcın değerlendirmesi. <see cref="Candidate"/>, ihlal yalnız uyarı sınıfındaysa da (personel sabit)
/// doludur; böylece panel ikinci onayla (<c>allowOutsideWorkingHours</c>) yazabilir.
/// </summary>
internal sealed record SlotEvaluation(Instant Start, SlotViolation Violations, SlotCandidate? Candidate)
{
    private const SlotViolation Overridable = SlotViolation.OutsideWorkingHours | SlotViolation.StaffOnTimeOff;

    public bool IsAvailable => Violations == SlotViolation.None && Candidate is not null;

    /// <summary>Çalışma saati ve izin ihlali panelde ikinci onayla geçilir; çakışma asla (musaitlik.md § Panelden elle ekleme).</summary>
    public bool IsAllowed(bool allowOutsideWorkingHours) =>
        Candidate is not null && (Violations == SlotViolation.None || (allowOutsideWorkingHours && (Violations & ~Overridable) == SlotViolation.None));
}
