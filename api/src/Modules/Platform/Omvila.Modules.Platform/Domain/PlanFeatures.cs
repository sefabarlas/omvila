namespace Omvila.Modules.Platform.Domain;

/// <summary>Plan özellik bayrakları (alan-modeli.md § Plan.Features, jsonb; sözleşme PlanFeatures). Ekran metinleri istemcidedir.</summary>
internal sealed record PlanFeatures(bool WalkInQueue, bool Waitlist, bool Packages, bool CrossBranchReports, bool PrioritySupport)
{
    public static PlanFeatures None { get; } = new(false, false, false, false, false);
}
