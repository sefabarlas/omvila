import { formatDate } from '@omvila/ui-web/format';
import { addDays, daysBetween } from '@omvila/ui-web/time';
import type { WaitlistTimeWindow } from './api';

export const timeWindowOptions: readonly { value: WaitlistTimeWindow; label: string }[] = [
  { value: 'Morning', label: 'Sabah' },
  { value: 'Afternoon', label: 'Öğleden sonra' },
  { value: 'Evening', label: 'Akşam' },
];

const windowText: Record<WaitlistTimeWindow, string> = {
  Morning: 'sabah',
  Afternoon: 'öğleden sonra',
  Evening: 'akşam',
};

/**
 * "Tercih" kolonu: ardışık günler tek aralık ("Per–Cum"), ayrık günler virgülle; saat dilimleri ardından
 * ("Per–Cum · sabah"). Gün yoksa "Herhangi bir gün", saat dilimi yoksa her saat uyar (yazılmaz).
 */
export function preferenceLabel(dates: readonly string[], windows: readonly WaitlistTimeWindow[]) {
  const sorted = [...dates].sort();
  const runs: string[][] = [];
  for (const date of sorted) {
    const run = runs.at(-1);
    if (run && addDays(run.at(-1) ?? date, 1) === date) {
      run.push(date);
    } else {
      runs.push([date]);
    }
  }

  const day = (date: string) => formatDate(date, 'weekdayShort');
  const days =
    runs.length === 0
      ? 'Herhangi bir gün'
      : runs
          .map((run) =>
            run.length === 1 ? day(run[0] ?? '') : `${day(run[0] ?? '')}–${day(run.at(-1) ?? '')}`,
          )
          .join(', ');
  const ordered = timeWindowOptions.filter((o) => windows.includes(o.value));
  return ordered.length === 0 || ordered.length === timeWindowOptions.length
    ? days
    : `${days} · ${ordered.map((o) => windowText[o.value]).join(' veya ')}`;
}

/** "3 gündür bekliyor"; aynı gün eklendiyse "bugün eklendi". */
export function waitingSinceLabel(joinedDate: string, today: string): string {
  const days = daysBetween(joinedDate, today);
  return days <= 0 ? 'bugün eklendi' : `${String(days)} gündür bekliyor`;
}

/** KPI alt satırı: "en eskisi 6 gündür"; aynı gün "en eskisi bugün eklendi". */
export function oldestLabel(joinedDate: string, today: string): string {
  const days = daysBetween(joinedDate, today);
  return days <= 0 ? 'en eskisi bugün eklendi' : `en eskisi ${String(days)} gündür`;
}

/** Tabloda personelin ilk adı ("Elif"); tercih yoksa "Fark etmez". */
export function staffFirstName(fullName: string | null | undefined): string {
  return fullName ? (fullName.split(' ')[0] ?? fullName) : 'Fark etmez';
}
