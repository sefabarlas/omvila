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
