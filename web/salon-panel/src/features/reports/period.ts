// Raporlar ve Primim için dönem, etiket ve grafik ekseni yardımcıları. Günler şubenin yerel takvim günüdür
// (yyyy-aa-gg); cihazın saat dilimi kullanılmaz.
import { formatDate, formatMoney } from '@omvila/ui-web/format';
import { addDays } from '@omvila/ui-web/time';
import { weekStart } from '@/features/calendar/calendar-view';

export type ReportPeriod = 'gun' | 'hafta' | 'ay';

export const periodOptions = [
  { value: 'gun', label: 'Gün' },
  { value: 'hafta', label: 'Hafta' },
  { value: 'ay', label: 'Ay' },
] as const satisfies readonly { value: ReportPeriod; label: string }[];

export interface PeriodRange {
  /** Sorgu aralığı: dönemin ilk günü – bugün (iki uç dahil). */
  from: string;
  to: string;
  /** Dönemin son günü (grafikte gelecek günler boş sütunla çizilir). */
  end: string;
}

/** Ayın son günü. */
function monthEnd(day: string): string {
  return addDays(`${addDays(`${day.slice(0, 7)}-28`, 4).slice(0, 7)}-01`, -1);
}

/**
 * Gün = bugün; Hafta = bu haftanın Pazartesi'si – bugün; Ay = ayın 1'i – bugün. Sorgu bugünde biter: değişim yüzdesi
 * yarım dönemi tam dönemle karşılaştırmasın (acik-sorular.md T-122).
 */
export function periodRange(period: ReportPeriod, today: string): PeriodRange {
  switch (period) {
    case 'gun':
      return { from: today, to: today, end: today };
    case 'hafta':
      return { from: weekStart(today), to: today, end: addDays(weekStart(today), 6) };
    case 'ay':
      return { from: `${today.slice(0, 7)}-01`, to: today, end: monthEnd(today) };
  }
}

/** "25 Kasım", "18–24 Kasım", "30 Kasım – 2 Aralık". */
export function periodLabel(from: string, to: string): string {
  if (from === to) {
    return formatDate(from, 'dayMonth');
  }

  return from.slice(0, 7) === to.slice(0, 7)
    ? `${formatDate(from, 'day')}–${formatDate(to, 'dayMonth')}`
    : `${formatDate(from, 'dayMonth')} – ${formatDate(to, 'dayMonth')}`;
}

/** İki uç dahil günler. */
export function daysBetweenInclusive(from: string, to: string): string[] {
  const days: string[] = [];
  for (let day = from; day <= to; day = addDays(day, 1)) {
    days.push(day);
  }

  return days;
}

/** "%46" (oran tam sayıya yuvarlanır; para değildir). */
export function formatPercent(value: number): string {
  return `%${String(Math.round(value))}`;
}

/** "+%9", "−%3". */
export function formatChange(value: number): string {
  const rounded = Math.round(value);
  return `${rounded < 0 ? '−' : '+'}%${String(Math.abs(rounded))}`;
}

/** Puan: "4,9". */
export function formatRating(value: number): string {
  return value.toFixed(1).replace('.', ',');
}

/** Eksen adımı (kuruş): en büyük değeri dört çizgiye bölen 1-2-5 katı. */
export function axisStep(maxMinor: number): number {
  if (maxMinor <= 0) {
    return 0;
  }

  const raw = maxMinor / 4;
  const magnitude = 10 ** Math.floor(Math.log10(raw));
  const step = [1, 2, 5, 10].find((m) => m * magnitude >= raw) ?? 10;
  return step * magnitude;
}

/** Eksen etiketi: "5B ₺" (bin lira), küçük değerde "500 ₺". */
export function axisLabel(amountMinor: number): string {
  const lira = amountMinor / 100;
  if (lira >= 1000) {
    const thousands = lira / 1000;
    const text = Number.isInteger(thousands)
      ? String(thousands)
      : thousands.toFixed(1).replace('.', ',');
    // Tutarla sembol arasında bölünmez boşluk (formatMoney ile aynı).
    return `${text}B\u00A0₺`;
  }

  return formatMoney({ amountMinor: Math.round(amountMinor), currency: 'TRY' });
}

/** "Pazartesi kapalı", "Pazartesi ve Pazar kapalı"; ikiden fazla gün adıysa "Kapalı günler çizgiyle işaretli". */
export function closedDaysLabel(closedDays: readonly string[]): string | null {
  const names = [...new Set(closedDays.map((day) => formatDate(day, 'weekday')))];
  if (names.length === 0) {
    return null;
  }

  if (names.length > 2) {
    return 'Kapalı günler çizgiyle işaretli';
  }

  return `${names.join(' ve ')} kapalı`;
}

/** "Bugün", "Dün", "23 Kas". */
export function relativeDay(day: string, today: string): string {
  if (day === today) {
    return 'Bugün';
  }

  return day === addDays(today, -1) ? 'Dün' : formatDate(day, 'dayMonthShort');
}
