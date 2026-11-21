// Onay bekleyen talebin kalan süresi (dilim-03: "1 sa 35 dk içinde yanıtlayın"). Süre iki anın farkıdır, saat
// dilimi gerektirmez. "Şimdi" çağırandan gelir: enjekte edilen saat veya kuyruğun generatedUtc'si (cihaz saati değil).
import { formatDuration } from '@omvila/ui-web/format';

/** Kalan tam dakika (yukarı yuvarlanır: 94 dk 10 sn → 95); süre dolmuşsa 0. */
export function remainingMinutes(holdExpiresUtc: string, now: Date | string): number {
  const nowMs = typeof now === 'string' ? Date.parse(now) : now.getTime();
  const ms = Date.parse(holdExpiresUtc) - nowMs;
  return ms > 0 ? Math.ceil(ms / 60_000) : 0;
}

/** Yan panel: "1 sa 35 dk içinde yanıtlayın." */
export function replyWithinLabel(minutes: number): string {
  return `${formatDuration(minutes)} içinde yanıtlayın.`;
}

/** Bento: "ilki 1 sa 35 dk içinde iptal olur". */
export function firstExpiresLabel(minutes: number): string {
  return `ilki ${formatDuration(minutes)} içinde iptal olur`;
}
