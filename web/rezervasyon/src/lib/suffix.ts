// Türkçe ekler (ünlü ve ünsüz uyumu): salon adına bulunma eki ("Kuaför Elif'te", "Spa Moda'da") ve sayıya iyelik eki
// ("10 seansın 7'si"). Saat ekleri @omvila/ui-web/format withTimeSuffix'tedir.

const vowels = 'aeıioöuü';
const backVowels = 'aıou';
const voiceless = 'fstkçşhp';

const ones = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz'];
const tens = ['', 'on', 'yirmi', 'otuz', 'kırk', 'elli', 'altmış', 'yetmiş', 'seksen', 'doksan'];

function lastVowel(word: string): string | undefined {
  return Array.from(word)
    .reverse()
    .find((char) => vowels.includes(char));
}

function highVowel(vowel: string): string {
  if ('aı'.includes(vowel)) return 'ı';
  if ('ei'.includes(vowel)) return 'i';
  return 'ou'.includes(vowel) ? 'u' : 'ü';
}

/**
 * Adın son sözcüğüne bulunma eki: "Kuaför Elif" → "Kuaför Elif'te", "Spa Moda" → "Spa Moda'da". Son sözcük harfle
 * bitmiyorsa veya ünlüsü yoksa (kısaltma, sayı) null: çağıran eksiz bir cümle kurar.
 */
export function withLocative(name: string): string | null {
  const word = name.trim().split(/\s+/).at(-1)?.toLocaleLowerCase('tr-TR') ?? '';
  const last = word.at(-1) ?? '';
  const vowel = lastVowel(word);
  if (!/\p{L}/u.test(last) || !vowel) return null;
  const consonant = voiceless.includes(last) ? 't' : 'd';
  return `${name.trim()}'${consonant}${backVowels.includes(vowel) ? 'a' : 'e'}`;
}

/** Sayının okunuşunun son sözcüğü: 7 → "yedi", 10 → "on", 100 → "yüz". */
function lastWord(n: number): string {
  if (n === 0) return 'sıfır';
  if (n % 10 !== 0) return ones[n % 10] ?? '';
  if (n % 100 !== 0) return tens[(n % 100) / 10] ?? '';
  return n % 1000 !== 0 ? 'yüz' : 'bin';
}

/** Sayıya iyelik eki: 7 → "7'si", 3 → "3'ü", 10 → "10'u", 6 → "6'sı". */
export function withPossessive(n: number): string {
  const word = lastWord(Math.abs(Math.trunc(n)));
  const buffer = vowels.includes(word.at(-1) ?? '') ? 's' : '';
  return `${String(n)}'${buffer}${highVowel(lastVowel(word) ?? 'e')}`;
}
