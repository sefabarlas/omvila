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
