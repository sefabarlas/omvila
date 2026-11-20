// Şube saat dilimiyle zaman (AGENTS.md § 5): "şimdi" yalnız buradaki saatten alınır (testte sabit saat verilir);
// takvim günü şubenin timeZone değerine göre hesaplanır, cihazın saat dilimi kullanılmaz.

/** Enjekte edilen saat. */
export interface Clock {
  now: () => Date;
}

/** Gerçek saat; uygulama kökünde bir kez verilir. */
export const systemClock: Clock = { now: () => new Date() };

/** Sabit saat (test ve önizleme). */
export function fixedClock(instantUtc: string): Clock {
  return { now: () => new Date(instantUtc) };
}

/** Anlık zamanın şubenin saat dilimindeki takvim günü: "2026-11-25". */
export function toBranchDate(instant: Date, timeZone: string): string {
  const parts = new Intl.DateTimeFormat('en-CA', {
    timeZone,
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).formatToParts(instant);
  const value = (type: string) => parts.find((p) => p.type === type)?.value ?? '';
  return `${value('year')}-${value('month')}-${value('day')}`;
}

/** Şubede bugün. */
export function branchToday(clock: Clock, timeZone: string): string {
  return toBranchDate(clock.now(), timeZone);
}

/** İki takvim günü arasındaki gün farkı (b - a); saat dilimi geçişlerinden etkilenmez. */
export function daysBetween(a: string, b: string): number {
  const utc = (localDate: string) => {
    const [y, m, d] = localDate.split('-').map(Number) as [number, number, number];
    return Date.UTC(y, m - 1, d);
  };
  return Math.round((utc(b) - utc(a)) / 86_400_000);
}
