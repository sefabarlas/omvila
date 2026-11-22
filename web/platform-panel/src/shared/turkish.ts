import { formatCount } from '@omvila/ui-web/format';

// Sayının belirtme hâli (Genel bakış: "9'u bu hafta bitiyor", "3'ü salt okunura geçmek üzere"). Salon panelindeki
// shared/turkish.ts accusativeCount ile aynı kural; üçüncü kullanımda ui-web/format'a taşınır (T-123).

const ones = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz'];
const tens = ['', 'on', 'yirmi', 'otuz', 'kırk', 'elli', 'altmış', 'yetmiş', 'seksen', 'doksan'];

/** Okunuşun son kelimesi: 0 → "sıfır", 40 → "kırk", 100 → "yüz", 1000 → "bin". */
function spokenLastWord(n: number): string {
  if (n === 0) {
    return 'sıfır';
  }

  if (n % 1_000_000 === 0) {
    return 'milyon';
  }

  if (n % 1000 === 0) {
    return 'bin';
  }

  if (n % 100 === 0) {
    return 'yüz';
  }

  const rest = n % 100;
  return rest % 10 === 0 ? (tens[rest / 10] ?? '') : (ones[rest % 10] ?? '');
}

function highVowel(vowel: string): string {
  if ('aı'.includes(vowel)) {
    return 'ı';
  }

  if ('ei'.includes(vowel)) {
    return 'i';
  }

  return 'ou'.includes(vowel) ? 'u' : 'ü';
}

/** 9 → "9'u", 3 → "3'ü", 40 → "40'ı", 6 → "6'yı", 1240 → "1.240'ı". */
export function accusativeCount(n: number): string {
  const word = spokenLastWord(Math.abs(Math.trunc(n)));
  const vowel = /[aıoueiöü](?=[^aıoueiöü]*$)/.exec(word)?.[0] ?? 'e';
  return `${formatCount(n)}'${/[aıoueiöü]$/.test(word) ? 'y' : ''}${highVowel(vowel)}`;
}
