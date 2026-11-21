import { useState } from 'react';
import { useClock, useNow } from '@/app/clock';

/**
 * Listenin "şimdi"si: sunucunun generatedUtc'si + yanıt geldiğinden beri enjekte saatte geçen süre. Kalan teklif
 * süresi ve bekleme dakikaları buna göre sayılır (sözleşme: cihaz saati kullanılmaz). Testte sabit saat geçen süreyi
 * sıfır tutar; değer generatedUtc olur.
 */
export function useServerNow(generatedUtc: string | undefined, intervalMs = 15_000): Date | null {
  const clock = useClock();
  const now = useNow(intervalMs);
  const [anchor, setAnchor] = useState<{ generatedUtc: string; atMs: number } | null>(null);
  if (generatedUtc !== undefined && anchor?.generatedUtc !== generatedUtc) {
    // Yeni yanıt: geçen süre bu andan sayılır (render sırasında durum güncelleme deseni).
    setAnchor({ generatedUtc, atMs: clock.now().getTime() });
  }

  if (generatedUtc === undefined) {
    return null;
  }

  const atMs = anchor?.generatedUtc === generatedUtc ? anchor.atMs : now.getTime();
  return new Date(Date.parse(generatedUtc) + Math.max(0, now.getTime() - atMs));
}

/** İki an arasındaki tam dakika (aşağı yuvarlanır; negatifse 0): "12 dk". */
export function minutesBetween(fromUtc: string, to: Date): number {
  return Math.max(0, Math.floor((to.getTime() - Date.parse(fromUtc)) / 60_000));
}
