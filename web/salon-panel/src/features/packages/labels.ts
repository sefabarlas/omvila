import { formatDate } from '@omvila/ui-web/format';
import { dative } from '@/shared/turkish';
import type { CustomerPackageFilter, PackageDefinition } from './api';

/**
 * "Süresi yaklaşan" penceresi: sunucunun ExpiringSoon süzgeci 30 gün içinde biten paketleri verir (sözleşme
 * manageListAllCustomerPackages, T-95). Değer panele gelmediği için yalnız metinde, tek yerde.
 */
export const expiringSoonDays = 30;

/** "Geçerlilik" seçenekleri (gün; 3 ay = 90, sözleşme örneği). */
export const validityOptions: readonly { days: number; label: string }[] = [
  { days: 30, label: '1 ay' },
  { days: 60, label: '2 ay' },
  { days: 90, label: '3 ay' },
  { days: 120, label: '4 ay' },
  { days: 180, label: '6 ay' },
  { days: 270, label: '9 ay' },
  { days: 365, label: '12 ay' },
];

/** 90 → "3 ay"; seçeneklerde olmayan süre gün olarak ("45 gün"). */
export function validityLabel(days: number): string {
  return validityOptions.find((o) => o.days === days)?.label ?? `${String(days)} gün`;
}

/** Yerel takvim gününe gün ekler (yyyy-aa-gg; saat dilimi yok). */
export function addDays(localDate: string, days: number): string {
  const [year = 0, month = 1, day = 1] = localDate.split('-').map(Number);
  return new Date(Date.UTC(year, month - 1, day + days)).toISOString().slice(0, 10);
}

const ones = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz'];
const tens = ['', 'on', 'yirmi', 'otuz', 'kırk', 'elli', 'altmış', 'yetmiş', 'seksen', 'doksan'];

/** Yılın okunuşunun son sözcüğü: 2027 → "yedi", 2030 → "otuz", 2100 → "yüz", 2000 → "bin". */
function yearLastWord(year: number): string {
  if (year % 10 !== 0) {
    return ones[year % 10] ?? '';
  }

  if (year % 100 !== 0) {
    return tens[Math.floor((year % 100) / 10)] ?? '';
  }

  return year % 1000 !== 0 ? 'yüz' : 'bin';
}

/** "2027-02-20" → "20 Şubat 2027'ye kadar" (PaketSat "Geçerlilik"). */
export function untilLabel(localDate: string): string {
  const year = Number(localDate.slice(0, 4));
  const word = yearLastWord(year);
  const suffix = dative(word, false).slice(word.length);
  return `${formatDate(localDate, 'dayMonth')} ${String(year)}'${suffix} kadar`;
}

/** "6 × Manikür, 2 × Kalıcı oje" (PaketSat "İçerik"). */
export function contentsLabel(items: PackageDefinition['items']): string {
  return items.map((i) => `${String(i.sessionCount)} × ${i.serviceName}`).join(', ');
}

/** Müşteri paketleri süzgeçleri. */
export const customerPackageFilters: readonly { value: CustomerPackageFilter; label: string }[] = [
  { value: 'Active', label: 'Etkin' },
  { value: 'ExpiringSoon', label: 'Süresi yaklaşan' },
  { value: 'Expired', label: 'Süresi dolan' },
  { value: 'All', label: 'Tümü' },
];
