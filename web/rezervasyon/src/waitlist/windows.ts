import type { TimeWindow } from './waitlist-api';

// Saat dilimleri (WaitlistTimeWindow). Aralıkların sınırı sunucudadır (WaitlistMorningEndHour,
// WaitlistAfternoonEndHour; şube yerel saati); arayüz yalnız adı gösterir.

export const timeWindows: readonly TimeWindow[] = ['Morning', 'Afternoon', 'Evening'];

export const timeWindowLabels: Record<TimeWindow, string> = {
  Morning: 'Sabah',
  Afternoon: 'Öğleden sonra',
  Evening: 'Akşam',
};

/** "Öğleden sonra, Akşam"; boşsa "Her saat". */
export function windowsLabel(selected: readonly TimeWindow[]): string {
  return selected.length === 0
    ? 'Her saat'
    : timeWindows
        .filter((period) => selected.includes(period))
        .map((period) => timeWindowLabels[period])
        .join(', ');
}
