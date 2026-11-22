import { formatCount, formatMoney } from '@omvila/ui-web/format';
import pricingJson from '@content/pricing.json';

// Fiyat ve plan bilgisi content/pricing.json'dan (tek kaynak, S-14). null değer "karar bekleniyor" demektir:
// köşeli parantezli yer tutucu sitede görünmez, yerine uydurmasız bir metin çıkar.

export type PlanFeature =
  { text: string } | { limit: number | null; unit: string; pendingText: string };

export interface Plan {
  code: string;
  name: string;
  audience: string;
  monthlyPriceMinor: number | null;
  recommended: boolean;
  features: PlanFeature[];
}

export interface CreditPackage {
  code: string;
  credits: number;
  priceMinor: number | null;
}

export interface Pricing {
  trialDays: number;
  pastDueGraceDays: number;
  cardPaymentEnabled: boolean;
  vatNote: string | null;
  smsCharactersPerCredit: number;
  currency: string;
  plans: Plan[];
  creditPackages: CreditPackage[];
}

export const pricing: Pricing = pricingJson;

export const pricePendingText = 'Fiyat yakında açıklanacak';

export function featureText(feature: PlanFeature): string {
  if ('text' in feature) return feature.text;
  return feature.limit === null
    ? feature.pendingText
    : `${formatCount(feature.limit)} ${feature.unit}`;
}

export function priceText(amountMinor: number | null, data: Pricing = pricing): string | null {
  return amountMinor === null ? null : formatMoney({ amountMinor, currency: data.currency });
}

/** Fiyat altındaki not: "aylık · KDV dahil"; KDV kararı yoksa yalnız "aylık". */
export function monthlyNote(data: Pricing = pricing): string {
  return data.vatNote ? `aylık · ${data.vatNote}` : 'aylık';
}

export function creditsLabel(credits: number): string {
  return `${formatCount(credits)} SMS`;
}

/** "14 gün ücretsiz" */
export function trialText(data: Pricing = pricing): string {
  return `${String(data.trialDays)} gün ücretsiz`;
}

/** SSS "Deneme bitince ne olur?" cevabı; kartla ödeme açık değilse (T-06) yalnız havale anılır. */
export function trialEndAnswer(data: Pricing = pricing): string {
  const method = data.cardPaymentEnabled ? 'kartla ya da havaleyle' : 'havaleyle';
  return (
    `Plan seçip ${method} ödersiniz. Ödeme yapılmazsa ${String(data.pastDueGraceDays)} gün hatırlatma yapılır, ` +
    'sonra panel salt okunur olur. Verileriniz silinmez; ödeme yapınca her şey kaldığı yerden açılır.'
  );
}
