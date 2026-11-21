// IBAN gösterimi: dörtlü gruplar ("TR12 0006 4000 0011 2345 6789 01"). Kopyalanan değer boşluksuzdur.

/** Boşlukları atar, büyük harfe çevirir: "tr12 0006…" → "TR120006…". */
export function normalizeIban(input: string): string {
  return input.replace(/\s+/g, '').toLocaleUpperCase('tr-TR');
}

/** "TR120006400000112345678901" → "TR12 0006 4000 0011 2345 6789 01". */
export function formatIban(iban: string): string {
  return normalizeIban(iban).replace(/(.{4})(?=.)/g, '$1 ');
}
