import { formatDate } from '@omvila/ui-web/format';
import type { BadgeTone } from '@omvila/ui-web/panel';
import type { SubscriptionDetail, SubscriptionInvoice } from './api';

// Abonelik ekranı metinleri (salon-paneli/AyarlarAbonelik). Fatura durumu sözleşme enum'undan.
export const invoiceStatus: Record<
  SubscriptionInvoice['status'],
  { label: string; tone: BadgeTone }
> = {
  Paid: { label: 'Ödendi', tone: 'basari' },
  Issued: { label: 'Kesildi', tone: 'bilgi' },
  Failed: { label: 'Başarısız', tone: 'hata' },
  Void: { label: 'İptal', tone: 'notr' },
};

const cycle: Record<SubscriptionDetail['billingCycle'], string> = {
  Monthly: 'aylık',
  Yearly: 'yıllık',
};

const statusWord: Record<SubscriptionDetail['status'], string> = {
  Trialing: 'deneme',
  Active: 'etkin',
  PastDue: 'ödeme gecikti',
  ReadOnly: 'salt okunur',
  Cancelled: 'iptal',
};

/** PLAN kartı alt satırı: "aylık · deneme". */
export function planDetail(detail: Pick<SubscriptionDetail, 'billingCycle' | 'status'>): string {
  return `${cycle[detail.billingCycle]} · ${statusWord[detail.status]}`;
}

/** ŞUBE kartı alt satırı: "1 dahil + 1 ek şube"; dahil sayısı boşsa sınırsızdır. */
export function branchDetail(branches: SubscriptionDetail['branches']): string {
  if (branches.included === null) {
    return 'sınırsız şube';
  }

  const included = `${String(branches.included)} dahil`;
  return branches.extra > 0 ? `${included} + ${String(branches.extra)} ek şube` : included;
}

/** PERSONEL kartı alt satırı: sınır tanımlı değilse yazılmaz (S-14; uydurma sınır yok). */
export function staffDetail(staff: SubscriptionDetail['staff']): string | undefined {
  return staff.limit === null ? undefined : `${String(staff.limit)} kişiye kadar dahil`;
}

/** Fatura dönemi: tam takvim ayıysa "Ekim 2026", değilse "1 Kas – 30 Kas 2026". */
export function invoicePeriod(invoice: Pick<SubscriptionInvoice, 'periodStart' | 'periodEnd'>) {
  const sameMonth = invoice.periodStart.slice(0, 7) === invoice.periodEnd.slice(0, 7);
  if (sameMonth && invoice.periodStart.endsWith('-01')) {
    return formatDate(invoice.periodStart, 'monthYear');
  }

  return `${formatDate(invoice.periodStart, 'dayMonthShort')} – ${formatDate(invoice.periodEnd, 'dayMonthShort')} ${invoice.periodEnd.slice(0, 4)}`;
}
