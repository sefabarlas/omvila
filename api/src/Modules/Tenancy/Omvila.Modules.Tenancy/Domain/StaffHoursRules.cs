using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Personel saatleri (alan-modeli.md § StaffWorkingHours): bir personelin farklı şubelerdeki aralıkları aynı anda
/// örtüşemez. Şubeler arası geçiş süresi burada değil, müsaitlikte uygulanır (musaitlik.md § 5).
/// "İşletme saatleriyle aynı" şube saatlerinin kopyasıdır; bağ tutulmaz (dilim-01).
/// Hata: validation_failed + days[n].intervals = overlaps_other_branch (sözleşme; acik-sorular.md T-16).
/// </summary>
internal static class StaffHoursRules
{
    /// <summary>
    /// Bir şubede kaydedilen saatleri personelin diğer şubelerdeki saatleriyle karşılaştırır. Sözleşme
    /// (manageSetStaffWorkingHours): çakışan gün <c>days[n].intervals = overlaps_other_branch</c>; n istekteki sıradır.
    /// </summary>
    public static Result CheckAgainstOtherBranches(IReadOnlyList<IsoDayOfWeek> requestDays, WeeklyHours edited, IEnumerable<WeeklyHours> otherBranches)
    {
        ArgumentNullException.ThrowIfNull(requestDays);
        ArgumentNullException.ThrowIfNull(edited);
        ArgumentNullException.ThrowIfNull(otherBranches);
        var others = otherBranches.ToList();
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        for (var n = 0; n < requestDays.Count; n++)
        {
            var intervals = edited.Days[requestDays[n]];
            if (others.Exists(o => o.Days.TryGetValue(requestDays[n], out var other) && intervals.Any(i => other.Any(i.Overlaps))))
            {
                errors[$"days[{n}].intervals"] = ["overlaps_other_branch"];
            }
        }

        return errors.Count > 0 ? Error.Validation(errors) : Result.Success();
    }
}
