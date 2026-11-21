// Saatten sonra gelen Türkçe ek, saatin okunuşuna göre (ünlü ve ünsüz uyumu):
// "20:00'ye kadar", "10:00'da açılır", "16:30'da biter", "Cuma 10:30'u seç", "13:00'te".
// Dakika sıfırsa saat ("yirmi"), değilse dakika ("otuz") okunur; son sözcüğün son ünlüsü ve son sesi eki belirler.

export type TimeSuffix = 'dative' | 'locative' | 'accusative' | 'ablative';

const ones = ['', 'bir', 'iki', 'üç', 'dört', 'beş', 'altı', 'yedi', 'sekiz', 'dokuz'];
const tens = ['', 'on', 'yirmi', 'otuz', 'kırk', 'elli'];
const backVowels = 'aıou';
const vowels = 'aeıioöuü';
const voiceless = 'fstkçşhp';

/** Sayının okunuşunun son sözcüğü: 0 → "sıfır", 13 → "üç", 20 → "yirmi". */
function lastWord(n: number): string {
  if (n === 0) {
    return 'sıfır';
  }

  const one = n % 10;
  return one === 0 ? (tens[Math.floor(n / 10)] ?? '') : (ones[one] ?? '');
}

function highVowel(lastVowel: string): string {
  if ('aı'.includes(lastVowel)) {
    return 'ı';
  }

  if ('ei'.includes(lastVowel)) {
    return 'i';
  }

  return 'ou'.includes(lastVowel) ? 'u' : 'ü';
}

function suffixFor(word: string, kind: TimeSuffix): string {
  const lastVowel =
    Array.from(word)
      .reverse()
      .find((c) => vowels.includes(c)) ?? 'e';
  const back = backVowels.includes(lastVowel);
  const lastChar = word.at(-1) ?? '';
  const buffer = vowels.includes(lastChar) ? 'y' : '';
  const consonant = voiceless.includes(lastChar) ? 't' : 'd';
  switch (kind) {
    case 'dative':
      return `${buffer}${back ? 'a' : 'e'}`;
    case 'locative':
      return `${consonant}${back ? 'a' : 'e'}`;
    case 'ablative':
      return `${consonant}${back ? 'an' : 'en'}`;
    case 'accusative':
      return `${buffer}${highVowel(lastVowel)}`;
  }
}

/** "20:00" + dative → "20:00'ye"; "10:30" + accusative → "10:30'u"; "13:00" + locative → "13:00'te". */
export function withTimeSuffix(localTime: string, kind: TimeSuffix): string {
  const match = /^(\d{2}):(\d{2})$/.exec(localTime);
  if (!match) {
    throw new RangeError(`Saat ss:dd olmalı: ${localTime}`);
  }

  const [hour, minute] = [Number(match[1]), Number(match[2])];
  return `${localTime}'${suffixFor(lastWord(minute === 0 ? hour : minute), kind)}`;
}
