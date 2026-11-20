import { daysBetween } from '@omvila/ui-web/time';
import type { components } from '@omvila/api-client-ts';

export type WeeklyHours = components['schemas']['WeeklyHours'];
export type WorkingHoursOverride = components['schemas']['WorkingHoursOverride'];

export interface IntervalDraft {
  start: string;
  end: string;
}

export interface DayDraft {
  /** ISO gün: 1 = Pazartesi … 7 = Pazar. */
  dayOfWeek: number;
  open: boolean;
  intervals: IntervalDraft[];
}

export const dayNames = ['Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi', 'Pazar'];

/** Sözleşme: günde en fazla 4 aralık (WeeklyHours). */
export const maxIntervalsPerDay = 4;

/** Sunucudaki haftalık saatler → yedi günlük taslak (listede olmayan gün kapalı). */
export function toDrafts(hours: WeeklyHours): DayDraft[] {
  return dayNames.map((_, index) => {
    const dayOfWeek = index + 1;
    const day = hours.days.find((d) => d.dayOfWeek === dayOfWeek);
    return {
      dayOfWeek,
      open: day !== undefined,
      intervals: day ? day.intervals.map((i) => ({ start: i.start, end: i.end })) : [],
    };
  });
}

/** Taslak → gönderilecek haftalık saatler (kapalı gün listeye girmez). */
export function toWeeklyHours(drafts: readonly DayDraft[]): WeeklyHours {
  return {
    days: drafts
      .filter((d) => d.open)
      .map((d) => ({
        dayOfWeek: d.dayOfWeek,
        intervals: [...d.intervals].sort((a, b) => a.start.localeCompare(b.start)),
      })),
  };
}

/**
 * Gün açılınca önerilen aralıklar: en yakın önceki (yoksa sonraki) açık günün saatleri. Hiç açık gün yoksa boş bir
 * aralık (saatler kodda sabit yazılmaz; kullanıcı girer).
 */
export function suggestedIntervals(
  drafts: readonly DayDraft[],
  dayOfWeek: number,
): IntervalDraft[] {
  const open = drafts.filter((d) => d.open && d.intervals.length > 0);
  const before = [...open].reverse().find((d) => d.dayOfWeek < dayOfWeek);
  const source = before ?? open.find((d) => d.dayOfWeek > dayOfWeek);
  return source ? source.intervals.map((i) => ({ ...i })) : [{ start: '', end: '' }];
}

/**
 * Bir günün hatası (kabul kriteri 5): eksik saat, gece yarısını aşan veya ters aralık, örtüşen aralıklar.
 * Sunucu da aynı kuralları doğrular (validation_failed).
 */
export function dayError(day: DayDraft): string | undefined {
  if (!day.open) {
    return undefined;
  }

  if (day.intervals.length === 0 || day.intervals.some((i) => i.start === '' || i.end === '')) {
    return 'Başlangıç ve bitiş saatini girin.';
  }

  if (day.intervals.some((i) => i.end <= i.start)) {
    return 'Bitiş saati başlangıçtan sonra olmalı; aralık gece yarısını aşamaz.';
  }

  const sorted = [...day.intervals].sort((a, b) => a.start.localeCompare(b.start));
  for (let index = 1; index < sorted.length; index++) {
    const previous = sorted[index - 1];
    const current = sorted[index];
    if (previous && current && current.start < previous.end) {
      return 'Aralıklar örtüşmemeli.';
    }
  }

  return undefined;
}

/** "10:00–13:00, 14:00–20:00" */
export function formatIntervals(intervals: readonly IntervalDraft[]): string {
  return intervals.map((i) => `${i.start}–${i.end}`).join(', ');
}

export interface OverrideGroup {
  ids: string[];
  fromDate: string;
  toDate: string;
  dayCount: number;
  override: WorkingHoursOverride;
}

const sameRule = (a: WorkingHoursOverride, b: WorkingHoursOverride) =>
  a.kind === b.kind &&
  a.isClosed === b.isClosed &&
  (a.note ?? '') === (b.note ?? '') &&
  formatIntervals(a.intervals ?? []) === formatIntervals(b.intervals ?? []);

/**
 * Sunucu çok günlük kapanışı gün gün saklar; liste ardışık ve aynı kuraldaki günleri tek satırda gösterir
 * ("Ramazan Bayramı · 3 gün", kabul kriteri 6). Tarih sırasına göre.
 */
export function groupOverrides(items: readonly WorkingHoursOverride[]): OverrideGroup[] {
  const sorted = [...items].sort((a, b) => a.date.localeCompare(b.date));
  const groups: OverrideGroup[] = [];
  for (const item of sorted) {
    const last = groups.at(-1);
    if (last && daysBetween(last.toDate, item.date) === 1 && sameRule(last.override, item)) {
      last.ids.push(item.id);
      last.toDate = item.date;
      last.dayCount += 1;
    } else {
      groups.push({
        ids: [item.id],
        fromDate: item.date,
        toDate: item.date,
        dayCount: 1,
        override: item,
      });
    }
  }

  return groups;
}

/** Sağdaki tür etiketi: "Resmi tatil · kapalı", "Kapalı", "Özel saat", "Ek açılış". */
export function overrideKindLabel(override: WorkingHoursOverride): string {
  switch (override.kind) {
    case 'Holiday':
      return override.isClosed ? 'Resmi tatil · kapalı' : 'Resmi tatil · özel saat';
    case 'Closure':
      return 'Kapalı';
    case 'ShortDay':
      return 'Özel saat';
    case 'ExtraOpening':
      return 'Ek açılış';
  }
}
