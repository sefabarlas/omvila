using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Personel saatleri (alan-modeli.md § StaffWorkingHours): bir personelin farklı şubelerdeki aralıkları aynı anda
/// örtüşemez. Şubeler arası geçiş süresi burada değil, müsaitlikte uygulanır (musaitlik.md § 5).
/// "İşletme saatleriyle aynı" şube saatlerinin kopyasıdır; bağ tutulmaz (dilim-01).
/// Hata: validation_failed + workingHours alanı (acik-sorular.md T-16).
/// </summary>
internal static class StaffHoursRules
{
    public static Result CheckNoCrossBranchOverlap(IReadOnlyDictionary<Guid, WeeklyHours> hoursByBranch)
    {
        ArgumentNullException.ThrowIfNull(hoursByBranch);
        var branches = hoursByBranch.ToList();
        for (var a = 0; a < branches.Count; a++)
        {
            for (var b = a + 1; b < branches.Count; b++)
            {
                foreach (var (day, intervals) in branches[a].Value.Days)
                {
                    if (branches[b].Value.Days.TryGetValue(day, out var other) && intervals.Any(i => other.Any(i.Overlaps)))
                    {
                        return Error.Validation("workingHours", "overlapping_branches");
                    }
                }
            }
        }

        return Result.Success();
    }
}
