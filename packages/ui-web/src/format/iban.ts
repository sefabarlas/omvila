// IBAN gösterimi: dörtlü gruplar ("TR12 0006 4000 0011 2345 6789 01"). Kopyalanan değer boşluksuzdur.

/** Boşlukları atar, büyük harfe çevirir: "tr12 0006…" → "TR120006…". */
export function normalizeIban(input: string): string {
  return input.replace(/\s+/g, '').toLocaleUpperCase('tr-TR');
}

/** "TR120006400000112345678901" → "TR12 0006 4000 0011 2345 6789 01". */
export function formatIban(iban: string): string {
  return normalizeIban(iban).replace(/(.{4})(?=.)/g, '$1 ');
}

/**
 * Türkiye IBAN'ı mı: TR + 24 hane (toplam 26 karakter) ve ISO 13616 mod-97 denetimi 1.
 * Boşluklu ve küçük harfli giriş kabul edilir; sunucu aynı denetimi yapar (BankAccountWrite.iban).
 */
export function isValidIban(input: string): boolean {
  const iban = normalizeIban(input);
  if (!/^TR\d{24}$/.test(iban)) {
    return false;
  }

  // İlk dört karakter sona alınır, harfler sayıya çevrilir (T=29, R=27); kalan parça parça hesaplanır.
  const rearranged = `${iban.slice(4)}2927${iban.slice(2, 4)}`;
  let remainder = 0;
  for (const digit of rearranged) {
    remainder = (remainder * 10 + Number(digit)) % 97;
  }

  return remainder === 1;
}
