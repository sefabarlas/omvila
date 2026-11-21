// Takvim ölçüleri ve konum hesabı (panel README § Takvim): 1 saat = takvim-saat (64 px), 15 dk = 16 px.
// Konum şubenin saat dilimindeki duvar saatiyle hesaplanır; cihaz saat dilimi kullanılmaz.
import {
  formatClock,
  localDateTimeToUtc,
  localMinutesOfDay,
  parseClock,
  toBranchDate,
} from '../time';
import type { CalendarBlock, CalendarDropTarget } from './types';

/** --takvim-saat tokenının değeri; yalnız piksel → dakika dönüşümünde ölçü alınamazsa kullanılır. */
export const HOUR_HEIGHT_PX = 64;
/** Izgara adımı ve sürükleme yuvarlaması. */
export const SLOT_MINUTES = 15;
/** Bu süreden kısa bloklar tek satır (AppointmentBlock README). */
export const COMPACT_BELOW_MINUTES = 45;

const DAY_MINUTES = 24 * 60;

export interface GridRange {
  startMinutes: number;
  endMinutes: number;
}

/** Açılış–kapanış saatini tam saatlere genişletir: ("09:30", "18:15") → 09:00–19:00. */
export function gridRange(openTime: string, closeTime: string): GridRange {
  const open = parseClock(openTime);
  const close = parseClock(closeTime);
  const startMinutes = Math.floor(open / 60) * 60;
  const endMinutes = Math.min(DAY_MINUTES, Math.max(startMinutes + 60, Math.ceil(close / 60) * 60));
  return { startMinutes, endMinutes };
}

/** Izgaradaki saat etiketleri: 09:00, 10:00 … (son saat çizgisi etiketsiz). */
export function hourLabels(range: GridRange): string[] {
  const labels: string[] = [];
  for (let m = range.startMinutes; m < range.endMinutes; m += 60) {
    labels.push(formatClock(m));
  }

  return labels;
}

/** Dakika → piksel (64 px / saat). */
export function minutesToPx(minutes: number, hourHeightPx = HOUR_HEIGHT_PX): number {
  return (minutes * hourHeightPx) / 60;
}

/** Piksel → dakika. */
export function pxToMinutes(px: number, hourHeightPx = HOUR_HEIGHT_PX): number {
  return (px * 60) / hourHeightPx;
}

/** En yakın 15 dakikaya yuvarlar. */
export function snapMinutes(minutes: number, step = SLOT_MINUTES): number {
  return Math.round(minutes / step) * step;
}

/** Bir anın kolon gününe göre duvar saati dakikası; önceki gün → 0, sonraki gün → 1440. */
function minutesOnDate(instantUtc: string, date: string, timeZone: string): number {
  const day = toBranchDate(new Date(instantUtc), timeZone);
  if (day < date) {
    return 0;
  }

  if (day > date) {
    return DAY_MINUTES;
  }

  return localMinutesOfDay(instantUtc, timeZone);
}

export interface BlockLayout {
  startMinutes: number;
  endMinutes: number;
  /** Izgara başından saat cinsinden (CSS: calc(var(--takvim-saat) * top)). */
  top: number;
  height: number;
  topPx: number;
  heightPx: number;
}

/** Bloğun kolon günündeki yeri; o güne düşmüyorsa veya ızgara dışındaysa null. */
export function blockLayout(
  block: Pick<CalendarBlock, 'startUtc' | 'endUtc'>,
  date: string,
  timeZone: string,
  range: GridRange,
): BlockLayout | null {
  const rawStart = minutesOnDate(block.startUtc, date, timeZone);
  const rawEnd = minutesOnDate(block.endUtc, date, timeZone);
  const startMinutes = Math.max(rawStart, range.startMinutes);
  const endMinutes = Math.min(rawEnd, range.endMinutes);
  if (endMinutes <= startMinutes || rawStart >= DAY_MINUTES) {
    return null;
  }

  const offset = startMinutes - range.startMinutes;
  const size = endMinutes - startMinutes;
  return {
    startMinutes,
    endMinutes,
    top: offset / 60,
    height: size / 60,
    topPx: minutesToPx(offset),
    heightPx: minutesToPx(size),
  };
}

export interface LaneInfo {
  lane: number;
  lanes: number;
}

/** Çakışan blokları yan yana şeritlere dağıtır (aynı kolonda üst üste binme olmasın). */
export function assignLanes(
  items: readonly { startMinutes: number; endMinutes: number }[],
): LaneInfo[] {
  const order = items
    .map((item, index) => ({ ...item, index }))
    .sort((a, b) => a.startMinutes - b.startMinutes || b.endMinutes - a.endMinutes);
  const result: LaneInfo[] = items.map(() => ({ lane: 0, lanes: 1 }));
  let cluster: { index: number; lane: number }[] = [];
  let laneEnds: number[] = [];
  let clusterEnd = -1;

  const closeCluster = () => {
    for (const member of cluster) {
      result[member.index] = { lane: member.lane, lanes: laneEnds.length };
    }

    cluster = [];
    laneEnds = [];
  };

  for (const item of order) {
    if (item.startMinutes >= clusterEnd) {
      closeCluster();
    }

    let lane = laneEnds.findIndex((end) => end <= item.startMinutes);
    if (lane === -1) {
      lane = laneEnds.length;
      laneEnds.push(item.endMinutes);
    } else {
      laneEnds[lane] = item.endMinutes;
    }

    cluster.push({ index: item.index, lane });
    clusterEnd = Math.max(clusterEnd, item.endMinutes);
  }

  closeCluster();
  return result;
}

/** Yerel gün + dakika → UTC an. */
export function slotStartUtc(date: string, minutesOfDay: number, timeZone: string): string {
  return localDateTimeToUtc(date, formatClock(minutesOfDay), timeZone);
}

export interface DropInput {
  block: Pick<CalendarBlock, 'appointmentId' | 'itemId' | 'staffId' | 'startUtc' | 'endUtc'>;
  /** Bloğun çizildiği kolon. */
  from: { staffId: string; date: string };
  /** Bırakılan kolon. */
  to: { staffId: string; date: string };
  /** Dikey kayma (px). */
  deltaY: number;
  timeZone: string;
  range: GridRange;
  hourHeightPx?: number;
}

/**
 * Sürüklemenin hedefi: başlangıç dikey kaymayla 15 dakikaya yuvarlanır, süre korunur, ızgara içinde kalır.
 */
export function computeDropTarget(input: DropInput): CalendarDropTarget {
  const { block, from, to, timeZone, range } = input;
  const startMinutes = minutesOnDate(block.startUtc, from.date, timeZone);
  const duration = Math.max(
    SLOT_MINUTES,
    Math.round((Date.parse(block.endUtc) - Date.parse(block.startUtc)) / 60_000),
  );
  const moved = snapMinutes(startMinutes + pxToMinutes(input.deltaY, input.hourHeightPx));
  const latestStart = Math.max(range.startMinutes, range.endMinutes - duration);
  const newStart = Math.min(Math.max(moved, range.startMinutes), latestStart);
  const newEnd = Math.min(newStart + duration, DAY_MINUTES);
  const startUtc = slotStartUtc(to.date, newStart, timeZone);
  return {
    appointmentId: block.appointmentId,
    itemId: block.itemId,
    staffId: to.staffId,
    date: to.date,
    startTime: formatClock(newStart),
    endTime: formatClock(newEnd),
    startUtc,
    endUtc: new Date(Date.parse(startUtc) + duration * 60_000).toISOString(),
    staffChanged: to.staffId !== from.staffId,
    timeChanged: startUtc !== new Date(block.startUtc).toISOString(),
  };
}
