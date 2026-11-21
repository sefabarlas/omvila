import { describe, expect, it } from 'vitest';
import {
  HOUR_HEIGHT_PX,
  assignLanes,
  blockLayout,
  computeDropTarget,
  gridRange,
  hourLabels,
  minutesToPx,
  pxToMinutes,
  slotStartUtc,
  snapMinutes,
} from './geometry';

// Kural: web/salon-panel/AGENTS.md § Takvim — 1 saat = 64 px, 15 dk = 16 px; takvim şubenin timeZone'u ile çizilir.
const IST = 'Europe/Istanbul';
const range = gridRange('09:00', '19:00');

describe('ölçü', () => {
  it('1 saat 64 px, 15 dakika 16 px', () => {
    expect(HOUR_HEIGHT_PX).toBe(64);
    expect(minutesToPx(60)).toBe(64);
    expect(minutesToPx(15)).toBe(16);
    expect(pxToMinutes(16)).toBe(15);
  });

  it('15 dakikaya yuvarlama', () => {
    expect(snapMinutes(607)).toBe(600);
    expect(snapMinutes(608)).toBe(615);
    expect(snapMinutes(-8)).toBe(-15);
  });

  it('ızgara tam saatlere genişler, etiketler saat başında', () => {
    expect(gridRange('09:30', '18:15')).toEqual({ startMinutes: 540, endMinutes: 1140 });
    expect(hourLabels(range)).toEqual([
      '09:00',
      '10:00',
      '11:00',
      '12:00',
      '13:00',
      '14:00',
      '15:00',
      '16:00',
      '17:00',
      '18:00',
    ]);
  });
});

describe('blok konumu', () => {
  it('İstanbul: 10:30–12:30 → üstten 96 px, 128 px boy', () => {
    const layout = blockLayout(
      { startUtc: '2026-11-25T07:30:00Z', endUtc: '2026-11-25T09:30:00Z' },
      '2026-11-25',
      IST,
      range,
    );
    expect(layout).toMatchObject({ startMinutes: 630, endMinutes: 750, topPx: 96, heightPx: 128 });
    expect(layout?.top).toBe(1.5);
    expect(layout?.height).toBe(2);
  });

  it('başka güne düşen blok çizilmez', () => {
    expect(
      blockLayout(
        { startUtc: '2026-11-26T07:30:00Z', endUtc: '2026-11-26T08:30:00Z' },
        '2026-11-25',
        IST,
        range,
      ),
    ).toBeNull();
  });

  it('ızgara dışına taşan kısım kırpılır', () => {
    const layout = blockLayout(
      { startUtc: '2026-11-25T05:30:00Z', endUtc: '2026-11-25T06:30:00Z' },
      '2026-11-25',
      IST,
      range,
    );
    expect(layout).toMatchObject({ startMinutes: 540, endMinutes: 570, topPx: 0, heightPx: 32 });
  });

  it('Berlin yaz saati geçiş günü: 10:00 duvar saatiyle çizilir', () => {
    // 29 Mart 2026: 02:00 → 03:00. 10:00 CEST = 08:00Z.
    const dst = blockLayout(
      { startUtc: '2026-03-29T08:00:00Z', endUtc: '2026-03-29T09:00:00Z' },
      '2026-03-29',
      'Europe/Berlin',
      range,
    );
    const before = blockLayout(
      { startUtc: '2026-03-28T09:00:00Z', endUtc: '2026-03-28T10:00:00Z' },
      '2026-03-28',
      'Europe/Berlin',
      range,
    );
    expect(dst?.topPx).toBe(64);
    expect(before?.topPx).toBe(64);
    expect(slotStartUtc('2026-03-29', 600, 'Europe/Berlin')).toBe('2026-03-29T08:00:00.000Z');
    expect(slotStartUtc('2026-03-28', 600, 'Europe/Berlin')).toBe('2026-03-28T09:00:00.000Z');
  });
});

it('çakışan bloklar yan yana şeritlere ayrılır', () => {
  expect(
    assignLanes([
      { startMinutes: 600, endMinutes: 660 },
      { startMinutes: 630, endMinutes: 690 },
      { startMinutes: 700, endMinutes: 730 },
    ]),
  ).toEqual([
    { lane: 0, lanes: 2 },
    { lane: 1, lanes: 2 },
    { lane: 0, lanes: 1 },
  ]);
});

describe('bırakma hedefi', () => {
  const block = {
    appointmentId: 'a1',
    itemId: 'i1',
    staffId: 'elif',
    startUtc: '2026-11-25T10:00:00Z', // 13:00
    endUtc: '2026-11-25T10:45:00Z',
  };
  const from = { staffId: 'elif', date: '2026-11-25' };

  it('başka personele ve 64 px aşağı: 14:00–14:45, süre korunur', () => {
    const target = computeDropTarget({
      block,
      from,
      to: { staffId: 'zeynep', date: '2026-11-25' },
      deltaY: 64,
      timeZone: IST,
      range,
    });
    expect(target).toMatchObject({
      staffId: 'zeynep',
      startTime: '14:00',
      endTime: '14:45',
      startUtc: '2026-11-25T11:00:00.000Z',
      endUtc: '2026-11-25T11:45:00.000Z',
      staffChanged: true,
      timeChanged: true,
    });
  });

  it('kayma 15 dakikaya yuvarlanır (21 px → 15 dk, 25 px → 30 dk)', () => {
    const to = from;
    expect(computeDropTarget({ block, from, to, deltaY: 21, timeZone: IST, range }).startTime).toBe(
      '13:15',
    );
    expect(computeDropTarget({ block, from, to, deltaY: 25, timeZone: IST, range }).startTime).toBe(
      '13:30',
    );
  });

  it('yerinde bırakma değişiklik sayılmaz', () => {
    const target = computeDropTarget({ block, from, to: from, deltaY: 3, timeZone: IST, range });
    expect(target.staffChanged).toBe(false);
    expect(target.timeChanged).toBe(false);
  });

  it('ızgara dışına çıkamaz', () => {
    const late = computeDropTarget({ block, from, to: from, deltaY: 2000, timeZone: IST, range });
    expect(late.startTime).toBe('18:15');
    expect(late.endTime).toBe('19:00');
    const early = computeDropTarget({ block, from, to: from, deltaY: -2000, timeZone: IST, range });
    expect(early.startTime).toBe('09:00');
  });

  it('hafta görünümü: başka güne taşıma', () => {
    const target = computeDropTarget({
      block,
      from,
      to: { staffId: 'elif', date: '2026-11-27' },
      deltaY: 0,
      timeZone: IST,
      range,
    });
    expect(target.startUtc).toBe('2026-11-27T10:00:00.000Z');
    expect(target.staffChanged).toBe(false);
    expect(target.timeChanged).toBe(true);
  });

  it('ölçülen saat yüksekliği kullanılabilir', () => {
    const target = computeDropTarget({
      block,
      from,
      to: from,
      deltaY: 128,
      timeZone: IST,
      range,
      hourHeightPx: 128,
    });
    expect(target.startTime).toBe('14:00');
  });
});
