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

/** Anlık zamanın şubenin saat dilimindeki saati (0–23); selamlama gibi gün içi metinler için. */
export function branchHour(clock: Clock, timeZone: string): number {
  const hour = new Intl.DateTimeFormat('en-GB', { timeZone, hour: '2-digit', hourCycle: 'h23' })
    .formatToParts(clock.now())
    .find((p) => p.type === 'hour')?.value;
  return Number(hour ?? '0');
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

/** Saat diliminin verilen andaki UTC farkı (dakika): Europe/Istanbul → 180. */
function offsetMinutes(instant: Date, timeZone: string): number {
  const parts = new Intl.DateTimeFormat('en-US', {
    timeZone,
    hourCycle: 'h23',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).formatToParts(instant);
  const value = (type: string) => Number(parts.find((p) => p.type === type)?.value ?? '0');
  const asUtc = Date.UTC(
    value('year'),
    value('month') - 1,
    value('day'),
    value('hour'),
    value('minute'),
  );
  return Math.round((asUtc - Math.floor(instant.getTime() / 60_000) * 60_000) / 60_000);
}

/**
 * Şubenin yerel tarih ve saatini UTC ana çevirir: ("2026-12-02", "14:00", "Europe/Istanbul") →
 * "2026-12-02T11:00:00.000Z". Yaz saati geçişlerinde farkı o anki değerle yeniden hesaplar.
 */
export function localDateTimeToUtc(localDate: string, localTime: string, timeZone: string): string {
  const [y, m, d] = localDate.split('-').map(Number) as [number, number, number];
  const [hh, mm] = localTime.split(':').map(Number) as [number, number];
  const wall = Date.UTC(y, m - 1, d, hh, mm);
  let guess = wall - offsetMinutes(new Date(wall), timeZone) * 60_000;
  guess = wall - offsetMinutes(new Date(guess), timeZone) * 60_000;
  return new Date(guess).toISOString();
}

/**
 * Anlık zamanın şubenin saat dilimindeki duvar saati, gece yarısından dakika: ("2026-11-25T07:30:00Z",
 * "Europe/Istanbul") → 630 (10:30). Takvim ızgarası duvar saatiyle çizildiği için yaz saati geçiş gününde de
 * 10:00 her zaman 600'dür (geçen süre değil).
 */
export function localMinutesOfDay(instant: Date | string, timeZone: string): number {
  const parts = new Intl.DateTimeFormat('en-GB', {
    timeZone,
    hour: '2-digit',
    minute: '2-digit',
    hourCycle: 'h23',
  }).formatToParts(typeof instant === 'string' ? new Date(instant) : instant);
  const value = (type: string) => Number(parts.find((p) => p.type === type)?.value ?? '0');
  return value('hour') * 60 + value('minute');
}

/** "09:30" → 570. */
export function parseClock(localTime: string): number {
  const match = /^(\d{2}):(\d{2})$/.exec(localTime);
  const hours = Number(match?.[1]);
  const minutes = Number(match?.[2]);
  if (!match || hours > 24 || minutes > 59 || (hours === 24 && minutes > 0)) {
    throw new RangeError(`Saat ss:dd olmalı: ${localTime}`);
  }

  return hours * 60 + minutes;
}

/** 570 → "09:30". */
export function formatClock(minutesOfDay: number): string {
  if (!Number.isInteger(minutesOfDay) || minutesOfDay < 0 || minutesOfDay > 24 * 60) {
    throw new RangeError('Dakika 0–1440 arası tamsayı olmalı.');
  }

  const hours = Math.floor(minutesOfDay / 60);
  return `${String(hours).padStart(2, '0')}:${String(minutesOfDay % 60).padStart(2, '0')}`;
}

/** Takvim gününe gün ekler: ("2026-11-30", 1) → "2026-12-01". Saat diliminden bağımsızdır. */
export function addDays(localDate: string, days: number): string {
  const [y, m, d] = localDate.split('-').map(Number) as [number, number, number];
  return new Date(Date.UTC(y, m - 1, d + days)).toISOString().slice(0, 10);
}
