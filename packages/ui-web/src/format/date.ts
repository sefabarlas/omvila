// Tarih ve saat biçimleri (urun-kurallari.md § Biçimler). Takvim günü yerel tarih ("2026-11-21") olarak gelir;
// anlık zaman (UTC) yalnız şubenin saat dilimiyle saate çevrilir. Cihazın saat dilimi kullanılmaz.

const months = [
  'Ocak',
  'Şubat',
  'Mart',
  'Nisan',
  'Mayıs',
  'Haziran',
  'Temmuz',
  'Ağustos',
  'Eylül',
  'Ekim',
  'Kasım',
  'Aralık',
];
const shortMonths = [
  'Oca',
  'Şub',
  'Mar',
  'Nis',
  'May',
  'Haz',
  'Tem',
  'Ağu',
  'Eyl',
  'Eki',
  'Kas',
  'Ara',
];
// getUTCDay sırası: 0 = Pazar.
const weekdays = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
const shortWeekdays = ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'];

export type DateStyle =
  | 'long'
  | 'panel'
  | 'short'
  | 'numeric'
  | 'dayMonth'
  | 'compact'
  | 'weekdayDayMonth'
  | 'monthYear'
  | 'weekday'
  | 'weekdayShort'
  | 'day';

interface Parts {
  year: number;
  month: number;
  day: number;
  weekday: number;
}

function parseLocalDate(localDate: string): Parts {
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(localDate);
  if (!match) {
    throw new RangeError(`Yerel tarih yyyy-aa-gg olmalı: ${localDate}`);
  }

  const [year, month, day] = [Number(match[1]), Number(match[2]), Number(match[3])];
  const utc = new Date(Date.UTC(year, month - 1, day));
  if (utc.getUTCMonth() !== month - 1 || utc.getUTCDate() !== day) {
    throw new RangeError(`Geçersiz tarih: ${localDate}`);
  }

  return { year, month, day, weekday: utc.getUTCDay() };
}

/**
 * long "21 Kasım Cuma", panel "Salı, 25 Kasım", short "21 Kas Cum", numeric "21.11.2026", dayMonth "31 Aralık"
 * (salon-paneli/AyarlarSube özel günler). Rezervasyon sayfası: compact "Cum 28 Kas" (Randevum, Ertele alt çubuğu),
 * weekdayDayMonth "Cuma 28 Kasım", monthYear "Kasım 2026" (gün şeridi başlığı), weekday "Cuma", weekdayShort "Cum",
 * day "28".
 * @param localDate Takvim günü, yyyy-aa-gg.
 */
export function formatDate(localDate: string, style: DateStyle = 'long'): string {
  const p = parseLocalDate(localDate);
  const month = months[p.month - 1] ?? '';
  const weekday = weekdays[p.weekday] ?? '';
  switch (style) {
    case 'long':
      return `${String(p.day)} ${month} ${weekday}`;
    case 'panel':
      return `${weekday}, ${String(p.day)} ${month}`;
    case 'short':
      return `${String(p.day)} ${shortMonths[p.month - 1] ?? ''} ${shortWeekdays[p.weekday] ?? ''}`;
    case 'dayMonth':
      return `${String(p.day)} ${month}`;
    case 'numeric':
      return `${String(p.day).padStart(2, '0')}.${String(p.month).padStart(2, '0')}.${String(p.year)}`;
    case 'compact':
      return `${shortWeekdays[p.weekday] ?? ''} ${String(p.day)} ${shortMonths[p.month - 1] ?? ''}`;
    case 'weekdayDayMonth':
      return `${weekday} ${String(p.day)} ${month}`;
    case 'monthYear':
      return `${month} ${String(p.year)}`;
    case 'weekday':
      return weekday;
    case 'weekdayShort':
      return shortWeekdays[p.weekday] ?? '';
    case 'day':
      return String(p.day);
  }
}

/** Anlık zamanın şubenin saat dilimindeki saati, 24 saat: "09:30". */
export function formatTime(instantUtc: string, timeZone: string): string {
  const parts = new Intl.DateTimeFormat('en-GB', {
    timeZone,
    hour: '2-digit',
    minute: '2-digit',
    hourCycle: 'h23',
  }).formatToParts(new Date(instantUtc));
  const value = (type: string) => parts.find((p) => p.type === type)?.value ?? '00';
  return `${value('hour')}:${value('minute')}`;
}

/** Süre: "45 dk", "1 sa 30 dk", "2 sa". */
export function formatDuration(minutes: number): string {
  if (!Number.isInteger(minutes) || minutes < 0) {
    throw new RangeError('Süre negatif olmayan tamsayı dakika olmalı.');
  }

  const hours = Math.floor(minutes / 60);
  const rest = minutes % 60;
  if (hours === 0) {
    return `${String(rest)} dk`;
  }

  return rest === 0 ? `${String(hours)} sa` : `${String(hours)} sa ${String(rest)} dk`;
}
