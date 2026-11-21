using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>Marka ayarlarından onay politikası (sureler-ve-ayarlar.md: RequiresApproval, ApprovalTimeoutMinutes).</summary>
internal static class ApprovalSettings
{
    public static ApprovalPolicy Policy(this BrandAppointmentSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return new ApprovalPolicy(settings.RequiresApproval, Duration.FromMinutes(settings.ApprovalTimeoutMinutes));
    }
}
