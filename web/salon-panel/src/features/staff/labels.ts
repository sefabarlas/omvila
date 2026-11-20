import type { StaffSummary, WeeklyHours } from './api';

const shortDays = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];

/** Rol etiketi (roller-yetkiler.md). */
export function roleLabel(role: StaffSummary['role']): string {
  switch (role) {
    case 'Owner':
      return 'İşletme sahibi';
    case 'BranchManager':
      return 'Şube müdürü';
    case 'Staff':
      return 'Personel';
    default:
      return 'Hesabı yok';
  }
}

/**
 * Çalışma özeti (PersonelBos "Pzt–Cmt · 10:00–20:00"): ardışık günler aralık olarak; günler farklı saatteyse
 * "Sal–Cmt · farklı saatler". Hiç gün yoksa "Saat girilmedi".
 */
export function workingSummary(hours: WeeklyHours | undefined): string {
  const days = [...(hours?.days ?? [])].sort((a, b) => a.dayOfWeek - b.dayOfWeek);
  const first = days[0];
  const last = days.at(-1);
  if (!first || !last) {
    return 'Saat girilmedi';
  }

  const consecutive = days.every(
    (d, i) => i === 0 || d.dayOfWeek === (days[i - 1]?.dayOfWeek ?? 0) + 1,
  );
  const dayText =
    days.length === 1
      ? shortDays[first.dayOfWeek - 1]
      : consecutive
        ? `${shortDays[first.dayOfWeek - 1] ?? ''}–${shortDays[last.dayOfWeek - 1] ?? ''}`
        : days.map((d) => shortDays[d.dayOfWeek - 1]).join(', ');
  const span = (d: (typeof days)[number]) =>
    `${d.intervals[0]?.start ?? ''}–${d.intervals.at(-1)?.end ?? ''}`;
  const same = days.every((d) => span(d) === span(first));
  return `${dayText ?? ''} · ${same ? span(first) : 'farklı saatler'}`;
}
