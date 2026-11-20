using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Plan sınırları (alan-modeli.md § Plan: StaffLimit?, IncludedBranchCount, ExtraBranchPriceMinor). Değerler veriden
/// okunur (sureler-ve-ayarlar.md: [PLAN SINIRI]); boş = sınırsız. Aşım 403 forbidden + meta.planLimit (sözleşme).
/// </summary>
internal sealed record PlanLimits(int? StaffLimit, int? IncludedBranchCount, bool AllowsExtraBranches)
{
    public static PlanLimits Unlimited { get; } = new(null, null, AllowsExtraBranches: true);

    /// <summary>Personel sınırı kesin: aktif personel sayısı (sahip dahil) sınıra ulaştıysa yeni personel eklenemez.</summary>
    public Result CanAddStaff(int activeStaffCount) =>
        StaffLimit is { } limit && activeStaffCount >= limit
            ? Error.Forbidden(meta: Meta("staff", limit))
            : Result.Success();

    /// <summary>
    /// Dahil şube sayısı aşılırsa: planda ek şube ücreti varsa izin verilir ve şube ek şubedir ("Aboneliğinize
    /// 1 ek şube eklenir"), yoksa 403.
    /// </summary>
    public Result<bool> CanAddBranch(int branchCount)
    {
        if (IncludedBranchCount is not { } included || branchCount < included)
        {
            return false;
        }

        return AllowsExtraBranches ? true : Error.Forbidden(meta: Meta("branches", included));
    }

    private static Dictionary<string, object?> Meta(string resource, int limit) =>
        new(StringComparer.Ordinal) { ["planLimit"] = new Dictionary<string, object?>(StringComparer.Ordinal) { ["resource"] = resource, ["limit"] = limit } };
}
