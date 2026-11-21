import { formatCount } from '@omvila/ui-web/format';
// Türkçe ek yardımcıları (arayüz metinleri). Özel adlarda ek kesme işaretiyle ayrılır ve yazımda ünsüz
// yumuşaması yapılmaz (TDK: "Mehmet'e", "Zeynep'e"); cins adlarda ("Salı" → "Salıya") kesme işareti yoktur.

const backVowels = new Set(['a', 'ı', 'o', 'u']);

function lastVowel(word: string): string | undefined {
  return /[aıoueiöüâîû](?=[^aıoueiöüâîû]*$)/.exec(word.toLocaleLowerCase('tr'))?.[0];
}

function endsWithVowel(word: string): boolean {
  return /[aıoueiöüâîû]$/.test(word.toLocaleLowerCase('tr'));
}

/**
 * Yönelme hâli (-e/-a): "Zeynep Aksoy" → "Zeynep Aksoy'a", "Mert Demir" → "Mert Demir'e", "Elif Kaya" →
 * "Elif Kaya'ya". Ek son kelimenin son ünlüsüne uyar; ünlüyle biten kelimeye kaynaştırma "y" gelir.
 * @param properNoun Özel ad mı (kesme işareti); "Salı" gibi cins adlarda false.
 */
export function dative(word: string, properNoun = true): string {
  const trimmed = word.trim();
  const last = trimmed.split(/\s+/).at(-1) ?? '';
  const vowel = backVowels.has(lastVowel(last) ?? 'e') ? 'a' : 'e';
  const suffix = `${endsWithVowel(last) ? 'y' : ''}${vowel}`;
  return `${trimmed}${properNoun ? "'" : ''}${suffix}`;
}

const genitiveVowel: Record<string, string> = {
  a: 'ı',
  ı: 'ı',
  o: 'u',
  u: 'u',
  e: 'i',
  i: 'i',
  ö: 'ü',
  ü: 'ü',
};

/**
 * İlgi hâli (-ın/-in/-un/-ün): "Elif Kaya" → "Elif Kaya'nın", "Mert Demir" → "Mert Demir'in", "Can Yücel" →
 * "Can Yücel'in", "Zeynep Aksoy" → "Zeynep Aksoy'un". Ünlüyle biten kelimeye kaynaştırma "n" gelir.
 */
export function genitive(word: string): string {
  const trimmed = word.trim();
  const last = trimmed.split(/\s+/).at(-1) ?? '';
  const vowel = genitiveVowel[lastVowel(last) ?? 'e'] ?? 'i';
  return `${trimmed}'${endsWithVowel(last) ? 'n' : ''}${vowel}n`;
}

// Sayıların okunuşundaki son kelimeye göre bulunma eki: bir'de, üç'te, altı'da, on'da, kırk'ta…
const unitLocative = ['da', 'de', 'de', 'te', 'te', 'te', 'da', 'de', 'de', 'da'];
const tensLocative = ['da', 'da', 'de', 'da', 'ta', 'de'];

/**
 * Saatin bulunma hâli, okunuşa göre: "13:15" → "13:15'te", "13:00" → "13:00'te" (on üç), "12:30" → "12:30'da"
 * (otuz), "09:40" → "09:40'ta" (kırk). Dakika sıfırsa saatin okunuşu esas alınır.
 */
export function locativeTime(time: string): string {
  const [hours = 0, minutes = 0] = time.split(':').map(Number);
  const spoken = minutes === 0 ? hours : minutes;
  const suffix =
    spoken % 10 !== 0
      ? (unitLocative[spoken % 10] ?? 'de')
      : (tensLocative[Math.floor(spoken / 10)] ?? 'de');
  return `${time}'${suffix}`;
}

const ones = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz'];
const tens = ['', 'on', 'yirmi', 'otuz', 'kırk', 'elli', 'altmış', 'yetmiş', 'seksen', 'doksan'];

/** Sayının okunuşunun son kelimesi: 180 → "seksen", 1240 → "kırk", 200 → "yüz", 0 → "sıfır". */
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

/** Sayının yönelme hâli, okunuşa göre: 180 → "180'e", 1240 → "1.240'a", 6 → "6'ya", 200 → "200'e". */
export function dativeCount(n: number): string {
  const word = spokenLastWord(Math.abs(Math.trunc(n)));
  const vowel = backVowels.has(lastVowel(word) ?? 'e') ? 'a' : 'e';
  return `${formatCount(n)}'${endsWithVowel(word) ? 'y' : ''}${vowel}`;
}

function highVowel(vowel: string): string {
  if ('aı'.includes(vowel)) {
    return 'ı';
  }

  if ('eiâî'.includes(vowel)) {
    return 'i';
  }

  return 'ouû'.includes(vowel) ? 'u' : 'ü';
}

/** Sayının belirtme hâli, okunuşa göre: 38 → "38'i", 40 → "40'ı", 6 → "6'yı", 100 → "100'ü". */
export function accusativeCount(n: number): string {
  const word = spokenLastWord(Math.abs(Math.trunc(n)));
  const vowel = highVowel(lastVowel(word) ?? 'e');
  return `${formatCount(n)}'${endsWithVowel(word) ? 'y' : ''}${vowel}`;
}
