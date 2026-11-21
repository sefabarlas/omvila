// Tutar: binlik nokta, kuruş virgül, sembol sonda, araya boşluk (urun-kurallari.md § Biçimler): 1.250 ₺, 85,50 ₺.
// Para kuruş cinsinden tamsayıdır; ondalıklı sayıyla hesap yapılmaz.

const symbols: Record<string, string> = { TRY: '₺' };

export interface MoneyValue {
  amountMinor: number;
  currency: string;
}

function group(integer: bigint): string {
  return integer.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
}

/** "1.250 ₺"; kuruş varsa "85,50 ₺". */
export function formatMoney(value: MoneyValue): string {
  if (!Number.isSafeInteger(value.amountMinor)) {
    throw new RangeError('Tutar kuruş cinsinden tamsayı olmalı.');
  }

  const minor = BigInt(value.amountMinor);
  const negative = minor < 0n;
  const absolute = negative ? -minor : minor;
  const lira = absolute / 100n;
  const kurus = absolute % 100n;
  const amount = kurus === 0n ? group(lira) : `${group(lira)},${kurus.toString().padStart(2, '0')}`;
  return `${negative ? '-' : ''}${amount} ${symbols[value.currency] ?? value.currency}`;
}

/**
 * Kullanıcının yazdığı tutarı kuruşa çevirir: "450" → 45000, "1.250" → 125000, "85,50" → 8550, "1.250,5 ₺" → 125050.
 * Binlik ayırıcı nokta, kuruş ayırıcı virgüldür. Geçersiz veya negatifse null. Ondalıklı sayı ile hesap yapılmaz.
 */
export function parseMoneyInput(input: string): number | null {
  const text = input.replace(/₺|TL|\s/gi, '');
  const match = /^(\d{1,3}(?:\.\d{3})+|\d+)(?:,(\d{1,2}))?$/.exec(text);
  if (!match?.[1]) {
    return null;
  }

  const lira = Number(match[1].replaceAll('.', ''));
  const kurus = Number((match[2] ?? '').padEnd(2, '0'));
  const minor = lira * 100 + kurus;
  return Number.isSafeInteger(minor) ? minor : null;
}

/** Hizmetin fiyat gösterimi (PublicCatalog.priceDisplay). */
export type PriceDisplay = 'Fixed' | 'StartingFrom';

/** Fixed "450 ₺", StartingFrom "2.800 ₺'den" (ServiceItem README: aralıklı fiyat). */
export function formatPrice(value: MoneyValue, display: PriceDisplay = 'Fixed'): string {
  const amount = formatMoney(value);
  return display === 'StartingFrom' ? `${amount}'den` : amount;
}

/** Düzenleme alanına yazılacak tutar: 45000 → "450", 8550 → "85,50" (sembolsüz). */
export function moneyInputValue(amountMinor: number): string {
  return formatMoney({ amountMinor, currency: 'TRY' }).replace(/\s₺$/, '');
}
