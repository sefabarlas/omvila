// Ad baş harfleri (panel Avatar ve rezervasyon bileşenleri ortak kullanır).

/** Baş harfler Türkçe büyük harfle (İlker → İ), en çok iki: "Kuaför Elif" → "KE". */
export function initials(name: string): string {
  return name
    .trim()
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part.charAt(0).toLocaleUpperCase('tr-TR'))
    .join('');
}

// Kişinin kendi yazdığı ad soyad (alan-modeli.md § User, T-130; sözleşme SignupRequest.ownerFullName). Sunucudaki
// kurallarla aynıdır (api Identity `PersonName`): tek değer, ad ve soyad ayrılmaz.

export const personNameMinLength = 2;
export const personNameMaxLength = 120;

export type PersonNameError = 'required' | 'too_short' | 'too_long' | 'invalid_format';

// Latin harfleri (Türkçe dahil; U+00C0–U+024F, × ve ÷ hariç), kesme işareti (' ve ’), tire ve boşluk; harfle başlar.
const personNamePattern =
  /^[A-Za-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u024F][A-Za-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u024F'\u2019 -]*$/;

/** Baştaki ve sondaki boşluk kırpılır, aradaki boşluk dizileri tek boşluğa iner: "  Alya   Kaya " → "Alya Kaya". */
export function normalizePersonName(value: string): string {
  return value.normalize('NFC').trim().replace(/\s+/g, ' ');
}

/** Yazılan adın hata kodu (sunucunun alan hatalarıyla aynı kodlar); geçerliyse `null`. */
export function personNameError(value: string): PersonNameError | null {
  const name = normalizePersonName(value);
  if (name.length === 0) return 'required';
  if (name.length < personNameMinLength) return 'too_short';
  if (name.length > personNameMaxLength) return 'too_long';
  return personNamePattern.test(name) ? null : 'invalid_format';
}
