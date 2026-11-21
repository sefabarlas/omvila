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
