import { formatCount, formatMoney, moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import type { Plan } from './api';

/** Kartın fiyatı; S-14 çözülene kadar boş olabilir ("Fiyat belirlenmedi"; köşeli parantezli yer tutucu yazılmaz). */
export function planPriceText(plan: Plan): string {
  return plan.monthlyPrice ? `${formatMoney(plan.monthlyPrice)} / ay` : 'Fiyat belirlenmedi';
}

/** Kart maddeleri: sınırlar veriden, özellik bayraklarının metinleri istemcide (adminListPlans). */
export function planLines(plan: Plan): string[] {
  const lines: string[] = [];
  lines.push(
    plan.staffLimit === null ? 'Sınırsız personel' : `${formatCount(plan.staffLimit)} personel`,
  );
  if (plan.includedBranchCount === null) {
    lines.push('Sınırsız şube');
  } else {
    const branches = `${formatCount(plan.includedBranchCount)} şube`;
    lines.push(
      plan.extraBranchPrice
        ? `${branches} dahil, ek şube ${formatMoney(plan.extraBranchPrice)} / ay`
        : branches,
    );
  }

  if (plan.includedSmsCredits !== null && plan.includedSmsCredits > 0) {
    lines.push(`Ayda ${formatCount(plan.includedSmsCredits)} SMS dahil`);
  }

  const { walkInQueue, waitlist, packages, crossBranchReports, prioritySupport } = plan.features;
  if (walkInQueue && waitlist) lines.push('Kapıda sıra ve bekleme listesi');
  else if (walkInQueue) lines.push('Kapıda sıra');
  else if (waitlist) lines.push('Bekleme listesi');
  if (packages) lines.push('Paketler ve prim raporu');
  if (crossBranchReports) lines.push('Şubeler arası rapor');
  if (prioritySupport) lines.push('Öncelikli destek');
  if (!walkInQueue && !waitlist && !packages && !crossBranchReports && !prioritySupport) {
    lines.push('Online randevu ve hatırlatma', 'Havale ile kapora takibi');
  }

  return lines;
}

/** Düzenleme alanı: kuruştan metne; tanımsız boş. */
export const moneyField = (value: { amountMinor: number } | null): string =>
  value ? moneyInputValue(value.amountMinor) : '';

/** Metinden tutara; boş null, geçersiz undefined. */
export function parseMoneyField(
  text: string,
): { amountMinor: number; currency: 'TRY' } | null | undefined {
  if (text.trim() === '') return null;
  const minor = parseMoneyInput(text);
  return minor === null ? undefined : { amountMinor: minor, currency: 'TRY' };
}

/** Metinden sayıya; boş null, geçersiz undefined. */
export function parseCountField(text: string, min: number): number | null | undefined {
  const value = text.trim().replaceAll('.', '');
  if (value === '') return null;
  if (!/^\d+$/.test(value)) return undefined;
  const n = Number(value);
  return n >= min ? n : undefined;
}
