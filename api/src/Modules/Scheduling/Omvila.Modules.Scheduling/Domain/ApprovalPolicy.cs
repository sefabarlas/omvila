using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Markanın onay ayarları (sureler-ve-ayarlar.md: <c>RequiresApproval</c> marka ve hizmet, <c>ApprovalTimeoutMinutes</c>).
/// "Onay gerekli" = <c>Service.RequiresApproval ?? BrandSettings.RequiresApproval</c>; kalemlerden biri onay istiyorsa
/// bütün randevu onay ister (durum-makinesi.md § Oluşturma kuralı).
/// </summary>
/// <param name="BrandRequiresApproval">"Randevular onayıma kesinleşsin".</param>
/// <param name="Timeout">Yanıt süresi (ApprovalTimeoutMinutes).</param>
internal sealed record ApprovalPolicy(bool BrandRequiresApproval, Duration Timeout)
{
    /// <summary>Plan onay istiyorsa tutma süresi, istemiyorsa <c>null</c>.</summary>
    /// <param name="plan">Randevunun kalem planları.</param>
    // Kural: durum-makinesi.md § Oluşturma kuralı ("Onay gerekli" çözümü)
    public Duration? HoldFor(AppointmentPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return plan.Items.Any(i => i.Offer.RequiresApproval ?? BrandRequiresApproval) ? Timeout : null;
    }
}
