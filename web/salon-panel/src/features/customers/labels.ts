import { formatDate, formatPhone, formatTime } from '@omvila/ui-web/format';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { addDays, toBranchDate } from '@omvila/ui-web/time';
import type { CustomerFilter, CustomerSummary } from './api';

/**
 * Filtre çipleri (salon-paneli/Musteriler). "Paketi olan" (WithPackage) paketler gelene kadar (D5) gizli: sunucu boş
 * döner (T-46 (4), T-57). Adreste Türkçe değer: ?filtre=duzenli.
 */
export const filterOptions: readonly { value: CustomerFilter; slug: string; label: string }[] = [
  { value: 'All', slug: 'tumu', label: 'Tümü' },
  { value: 'Regular', slug: 'duzenli', label: 'Düzenli' },
  { value: 'New', slug: 'yeni', label: 'Yeni' },
  { value: 'NoShowRisk', slug: 'gelmeme-riski', label: 'Gelmeme riski' },
  { value: 'Lapsed60', slug: '60-gun', label: '60+ gündür gelmeyen' },
];

export function filterFromSlug(slug: string | undefined): CustomerFilter {
  return filterOptions.find((option) => option.slug === slug)?.value ?? 'All';
}

export function filterSlug(filter: CustomerFilter): string | undefined {
  return filter === 'All' ? undefined : filterOptions.find((o) => o.value === filter)?.slug;
}

export function filterLabel(filter: CustomerFilter): string {
  return filterOptions.find((option) => option.value === filter)?.label ?? 'Tümü';
}

/** Etiket rengi: sunucunun sistem etiketleri (T-46 (4)); diğerleri nötr. */
export function tagTone(tag: string): BadgeTone {
  if (tag === 'Düzenli') {
    return 'marka';
  }

  if (tag === 'Yeni') {
    return 'vurgu';
  }

  return /gelmedi/i.test(tag) ? 'hata' : 'notr';
}

/** Telefon: tam numara yalnız sunucu verirse; personel rolünde maskeli ("0532 *** ** 21"). */
export function customerPhone(customer: Pick<CustomerSummary, 'phone' | 'phoneMasked'>): string {
  return customer.phone ? formatPhone(customer.phone) : (customer.phoneMasked ?? '');
}

/** Takvim günü: "Bugün", "Dün", aynı yıl "21 Ekim", başka yıl "21.10.2025". */
export function dayLabel(date: string, today: string): string {
  if (date === today) {
    return 'Bugün';
  }

  if (date === addDays(today, -1)) {
    return 'Dün';
  }

  if (date === addDays(today, 1)) {
    return 'Yarın';
  }

  return date.slice(0, 4) === today.slice(0, 4)
    ? formatDate(date, 'dayMonth')
    : formatDate(date, 'numeric');
}

/** Anlık zaman: "Bugün 13:00", "21 Ekim 14:00". */
export function instantLabel(instantUtc: string, today: string, timeZone: string): string {
  const date = toBranchDate(new Date(instantUtc), timeZone);
  return `${dayLabel(date, today)} ${formatTime(instantUtc, timeZone)}`;
}

/** Arama metni telefon gibi mi (yalnız rakam, boşluk, +). "Yeni müşteri olarak ekle" telefonu doldurur. */
export function looksLikePhone(query: string): boolean {
  return /^[\d\s+()-]+$/.test(query) && query.replace(/\D/g, '').length >= 10;
}
