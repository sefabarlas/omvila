import type { BadgeTone } from '@omvila/ui-web/panel';
import { formatTime } from '@omvila/ui-web/format';
import type { PaymentView } from '@/features/checkout/api';

// Kasa satırlarının metinleri (PaymentView açıklaması: "Kalem" istemcide üretilir).

/** Hizmet adları cümle içinde: "Boya ve fön", "Saç kesimi, fön ve maske". */
export function serviceList(names: readonly string[]): string {
  const words = names.map((name, i) => (i === 0 ? name : name.toLocaleLowerCase('tr')));
  if (words.length <= 1) {
    return words[0] ?? '';
  }

  return `${words.slice(0, -1).join(', ')} ve ${words.at(-1) ?? ''}`;
}

/**
 * "Kalem" kolonu: kapora → "Kapora · 14:30 randevusu"; randevu → "Boya ve fön" (kaporadan sonra kalan tutarsa
 * "· kalan"); paket satışı → "Fön paketi · 5 seans".
 */
export function paymentItemLabel(payment: PaymentView, timeZone: string): string {
  if (payment.purpose === 'CustomerPackage') {
    return [
      payment.packageName ?? 'Paket',
      payment.packageSessionCount ? `${String(payment.packageSessionCount)} seans` : null,
    ]
      .filter(Boolean)
      .join(' · ');
  }

  if (payment.isDeposit) {
    return payment.appointmentStartUtc
      ? `Kapora · ${formatTime(payment.appointmentStartUtc, timeZone)} randevusu`
      : 'Kapora';
  }

  const services = serviceList(payment.serviceNames ?? []) || 'Randevu';
  return payment.isBalanceAfterDeposit ? `${services} · kalan` : services;
}

/** Yöntem rozeti: kapora havalesi marka tonunda ayrışır (salon-paneli/Kasa). */
export function methodTone(payment: PaymentView): BadgeTone {
  return payment.isDeposit ? 'marka' : 'notr';
}
