// Sayı: binlik nokta (urun-kurallari.md § Biçimler): 1.240 kontör, 1.904 bildirim.

/** Tamsayıyı binlik noktayla yazar: 1240 → "1.240", 862 → "862". */
export function formatCount(value: number): string {
  if (!Number.isSafeInteger(value)) {
    throw new RangeError('Sayı tamsayı olmalı.');
  }

  const sign = value < 0 ? '-' : '';
  return (
    sign +
    Math.abs(value)
      .toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, '.')
  );
}
