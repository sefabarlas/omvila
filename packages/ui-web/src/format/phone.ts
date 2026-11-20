// Telefon: E.164 saklanır, "0532 123 45 67" gösterilir; maskeli "0532 *** ** 67" (AGENTS.md § 5, urun-kurallari.md).

function national(e164: string): string | null {
  const match = /^\+90(\d{10})$/.exec(e164);
  return match?.[1] ?? null;
}

/** "+905321234567" → "0532 123 45 67". Türkiye dışı veya biçimsiz değer olduğu gibi döner. */
export function formatPhone(e164: string): string {
  const n = national(e164);
  return n ? `0${n.slice(0, 3)} ${n.slice(3, 6)} ${n.slice(6, 8)} ${n.slice(8)}` : e164;
}

/** "+905321234567" → "0532 *** ** 67" (Personel rolü). */
export function maskPhone(e164: string): string {
  const n = national(e164);
  return n ? `0${n.slice(0, 3)} *** ** ${n.slice(8)}` : e164;
}

/**
 * Kullanıcının yazdığı cep telefonunu E.164'e çevirir: "0532 418 77 05", "532 418 77 05", "+90 532 418 77 05".
 * Cep telefonu değilse (5 ile başlamayan) null. Sunucu da ayrıca doğrular.
 */
export function parseMobilePhone(input: string): string | null {
  const digits = input.replace(/[\s()-]/g, '');
  const match = /^(?:\+90|0090|90|0)?(5\d{9})$/.exec(digits);
  return match?.[1] ? `+90${match[1]}` : null;
}
