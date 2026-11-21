import { formatDate, formatPhone, formatTime } from '@omvila/ui-web/format';
import type { BadgeTone } from '@omvila/ui-web/panel';
import { addDays, toBranchDate } from '@omvila/ui-web/time';
import type { CustomerFilter, CustomerPackage, CustomerSummary, PaymentView } from './api';

/**
 * Filtre çipleri (salon-paneli/Musteriler). "Paketi olan" (WithPackage) D5'te paketlerle açıldı (T-99). Adreste Türkçe
 * değer: ?filtre=duzenli.
 */
export const filterOptions: readonly { value: CustomerFilter; slug: string; label: string }[] = [
  { value: 'All', slug: 'tumu', label: 'Tümü' },
  { value: 'Regular', slug: 'duzenli', label: 'Düzenli' },
  { value: 'New', slug: 'yeni', label: 'Yeni' },
  { value: 'WithPackage', slug: 'paketi-olan', label: 'Paketi olan' },
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

/** Kısa gün: "31 Ara" (paket son günü, silme talebi tarihi). */
export function shortDate(localDate: string): string {
  return formatDate(localDate, 'short').split(' ').slice(0, 2).join(' ');
}

/**
 * Ödeme satırının kalemi (sözleşme PaymentView): kapora "Kapora · 10:30 randevusu", kalan "Boya, Fön · kalan", paket
 * satışı "Fön paketi · 5 seans", diğer randevu tahsilatı hizmet adları.
 */
export function paymentItemLabel(payment: PaymentView, timeZone: string): string {
  if (payment.purpose === 'CustomerPackage') {
    const name = payment.packageName ?? 'Paket';
    return payment.packageSessionCount
      ? `${name} · ${String(payment.packageSessionCount)} seans`
      : name;
  }

  const services = payment.serviceNames?.join(', ') ?? '';
  if (payment.isDeposit) {
    return payment.appointmentStartUtc
      ? `Kapora · ${formatTime(payment.appointmentStartUtc, timeZone)} randevusu`
      : 'Kapora';
  }

  if (payment.isBalanceAfterDeposit) {
    return services ? `${services} · kalan` : 'Kalan tutar';
  }

  return services || 'Randevu tahsilatı';
}

const paymentMethods: Record<string, string> = {
  Cash: 'Nakit',
  Card: 'Kart',
  Transfer: 'Havale',
  PackageRedemption: 'Paketten',
};

/** Yöntem: Nakit, Kart (salonun kendi POS'u), Havale, Paketten; bilinmeyen değer "Diğer" (sözleşme PaymentMethod). */
export function paymentMethodLabel(method: string): string {
  return paymentMethods[method] ?? 'Diğer';
}

/** İade ve iptal işareti (para hareketi yok, yalnız kayıt); kayıtlı ödemede rozet yok. */
export function paymentStatusBadge(
  status: PaymentView['status'],
): { label: string; tone: BadgeTone } | null {
  switch (status) {
    case 'PartiallyRefunded':
      return { label: 'Kısmen iade edildi', tone: 'uyari' };
    case 'Refunded':
      return { label: 'İade edildi', tone: 'uyari' };
    case 'Voided':
      return { label: 'İptal edildi', tone: 'notr' };
    default:
      return null;
  }
}

/** Paket durumu: Etkin, Süresi doldu, İade edildi, Devredildi. */
export function packageStatusBadge(status: CustomerPackage['status']): {
  label: string;
  tone: BadgeTone;
} {
  switch (status) {
    case 'Active':
      return { label: 'Etkin', tone: 'marka' };
    case 'Expired':
      return { label: 'Süresi doldu', tone: 'notr' };
    case 'Refunded':
      return { label: 'İade edildi', tone: 'uyari' };
    default:
      return { label: 'Devredildi', tone: 'notr' };
  }
}

/** Müşterinin ilk etkin paketi (liste Active önce gelir); paket kartı ve birleştirme özeti. */
export function activePackage(packages: readonly CustomerPackage[]): CustomerPackage | undefined {
  return packages.find((p) => p.status === 'Active');
}
