import type { components } from '@omvila/api-client-ts';
import { formatCount } from '@omvila/ui-web/format';
import type { BadgeTone } from '@omvila/ui-web/panel';

type Schemas = components['schemas'];
export type BrandListItem = Schemas['AdminBrandListItem'];
export type BrandDetail = Schemas['AdminBrandDetail'];
export type CreditTransaction = Schemas['AdminCreditTransaction'];
export type AttentionBrand = Schemas['AdminAttentionBrand'];
export type IndustryCode = Schemas['IndustryCode'];
export type SubscriptionStatus = Schemas['SubscriptionStatus'];

/** PMarkalar filtre çipleri (sözleşme: filter). */
export const brandFilters = [
  { value: 'All', label: 'Tümü' },
  { value: 'Trialing', label: 'Denemede' },
  { value: 'PastDue', label: 'Ödeme gecikti' },
  { value: 'ReadOnly', label: 'Salt okunur' },
  { value: 'DepositEnabled', label: 'Kapora açık' },
] as const;
export type BrandFilter = (typeof brandFilters)[number]['value'];

/** PMarkaDetay sekmeleri (URL: ?sekme=). */
export const brandTabs = [
  { value: 'ozet', label: 'Özet' },
  { value: 'fatura', label: 'Abonelik ve fatura' },
  { value: 'kontor', label: 'Kontör' },
  { value: 'odemeler', label: 'Ödemeler' },
  { value: 'denetim', label: 'Denetim kaydı' },
] as const;
export type BrandTab = (typeof brandTabs)[number]['value'];

// Platform listelerinde kısa adlar (PMarkalar: "Kuaför · İstanbul", "Güzellik · İstanbul").
const industryLabels: Record<IndustryCode, string> = {
  hair_salon: 'Kuaför',
  barber: 'Berber',
  beauty: 'Güzellik',
  nail: 'Tırnak',
  spa: 'Spa',
  dietitian: 'Diyetisyen',
  other: 'Diğer',
};

/** "Kuaför · İstanbul" (il yoksa yalnız tür). */
export function brandSubtitle(industryCode: IndustryCode, city: string | null): string {
  return [industryLabels[industryCode], city].filter(Boolean).join(' · ');
}

export const subscriptionLabels: Record<SubscriptionStatus, string> = {
  Trialing: 'Deneme',
  Active: 'Aktif',
  PastDue: 'Ödeme gecikti',
  ReadOnly: 'Salt okunur',
  Cancelled: 'İptal edildi',
};

const subscriptionTones: Record<SubscriptionStatus, BadgeTone> = {
  Trialing: 'notr',
  Active: 'basari',
  PastDue: 'uyari',
  ReadOnly: 'hata',
  Cancelled: 'notr',
};

/** Markalar tablosu "Durum": askıdaki marka abonelikten önce gelir; denemede kalan gün yazılır. */
export function brandStatusBadge(brand: BrandListItem): { label: string; tone: BadgeTone } {
  if (brand.brandStatus === 'Suspended') {
    return { label: 'Askıda', tone: 'hata' };
  }

  if (brand.brandStatus === 'Archived') {
    return { label: 'Arşivde', tone: 'notr' };
  }

  const status = brand.subscriptionStatus;
  const label =
    status === 'Trialing' && brand.trialDaysLeft !== null
      ? `Deneme · ${String(brand.trialDaysLeft)} gün`
      : subscriptionLabels[status];
  return { label, tone: subscriptionTones[status] };
}

/** "Dikkat isteyen markalar" nedeni. */
export function attentionBadge(brand: AttentionBrand): { label: string; tone: BadgeTone } {
  switch (brand.reason) {
    case 'PastDue':
      return {
        label:
          brand.pastDueDays === null
            ? 'Ödeme gecikti'
            : `Ödeme gecikti · ${String(brand.pastDueDays)} gün`,
        tone: 'uyari',
      };
    case 'CreditsDepleted':
      return { label: 'Kontör bitti', tone: 'hata' };
    case 'TrialEnding':
      return {
        label:
          brand.trialDaysLeft === null ? 'Deneme' : `Deneme · ${String(brand.trialDaysLeft)} gün`,
        tone: 'notr',
      };
  }
}

/** Kontör hareketi adı ("Paket satın alındı · 1.000 SMS", "Deneme hediyesi"). */
export function creditTransactionLabel(tx: CreditTransaction): string {
  switch (tx.type) {
    case 'Purchase':
      return `Paket satın alındı · ${formatCount(tx.credits)} SMS`;
    case 'Usage':
      return 'SMS gönderimi (günlük)';
    case 'TrialGift':
      return 'Deneme hediyesi';
    case 'Grant': {
      const kind = tx.credits < 0 ? 'Platform düzeltmesi' : 'Platform eklemesi';
      return tx.note ? `${kind} · ${tx.note}` : kind;
    }
    case 'Refund':
      return 'İade';
  }
}

/** "+1.000", "−46" (eksi işareti tipografik). */
export function signedCount(value: number): string {
  return value > 0 ? `+${formatCount(value)}` : value < 0 ? `−${formatCount(-value)}` : '0';
}

/** "Bu ay randevu": sıfır gerçek değerdir; veri yoksa çizgi (sahte sayı yok). */
export function countOrDash(value: number | null | undefined): string {
  return value === null || value === undefined ? '—' : formatCount(value);
}
