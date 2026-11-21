import { expect, it } from 'vitest';
import {
  addDays,
  branchHour,
  branchToday,
  daysBetween,
  fixedClock,
  formatClock,
  localDateTimeToUtc,
  localMinutesOfDay,
  parseClock,
  toBranchDate,
} from './index';

// Kural: web/salon-panel/AGENTS.md — takvim şubenin timeZone'u ile, duvar saatine göre çizilir.
it('duvar saati dakikası şubenin saat diliminde', () => {
  expect(localMinutesOfDay('2026-11-25T07:30:00Z', 'Europe/Istanbul')).toBe(630);
  expect(localMinutesOfDay(new Date('2026-11-25T21:30:00Z'), 'Europe/Istanbul')).toBe(30);
  // Berlin'de 29 Mart 2026 yaz saatine geçiş günü: 10:00 CEST = 08:00Z, bir gün önce 10:00 CET = 09:00Z.
  expect(localMinutesOfDay('2026-03-29T08:00:00Z', 'Europe/Berlin')).toBe(600);
  expect(localMinutesOfDay('2026-03-28T09:00:00Z', 'Europe/Berlin')).toBe(600);
});

it('saat metni ve dakika', () => {
  expect(parseClock('09:30')).toBe(570);
  expect(parseClock('24:00')).toBe(1440);
  expect(() => parseClock('9:30')).toThrow(RangeError);
  expect(() => parseClock('10:75')).toThrow(RangeError);
  expect(formatClock(570)).toBe('09:30');
  expect(formatClock(0)).toBe('00:00');
  expect(() => formatClock(-15)).toThrow(RangeError);
});

it('takvim gününe gün ekleme', () => {
  expect(addDays('2026-11-30', 1)).toBe('2026-12-01');
  expect(addDays('2026-03-28', 1)).toBe('2026-03-29');
  expect(addDays('2026-01-01', -1)).toBe('2025-12-31');
});

// Kural: AGENTS.md § 5 — takvim günü şubenin saat diliminde; cihaz saat dilimi yok.
it('gün şubenin saat dilimine göre', () => {
  // 25 Kasım 21:30 UTC = 26 Kasım 00:30 İstanbul.
  expect(toBranchDate(new Date('2026-11-25T21:30:00Z'), 'Europe/Istanbul')).toBe('2026-11-26');
  expect(toBranchDate(new Date('2026-11-25T21:30:00Z'), 'UTC')).toBe('2026-11-25');
  expect(branchToday(fixedClock('2026-11-25T09:00:00Z'), 'Europe/Istanbul')).toBe('2026-11-25');
});

it('gün farkı', () => {
  expect(daysBetween('2026-11-25', '2026-12-09')).toBe(14);
  expect(daysBetween('2026-03-28', '2026-03-30')).toBe(2);
  expect(daysBetween('2026-12-09', '2026-11-25')).toBe(-14);
});

it('şube saati', () => {
  expect(branchHour(fixedClock('2026-11-25T06:30:00Z'), 'Europe/Istanbul')).toBe(9);
  expect(branchHour(fixedClock('2026-11-25T21:30:00Z'), 'Europe/Istanbul')).toBe(0);
});

it('şube yerel saati UTC ana çevrilir', () => {
  expect(localDateTimeToUtc('2026-12-02', '14:00', 'Europe/Istanbul')).toBe(
    '2026-12-02T11:00:00.000Z',
  );
  expect(localDateTimeToUtc('2026-07-01', '09:30', 'Europe/Berlin')).toBe(
    '2026-07-01T07:30:00.000Z',
  );
  expect(localDateTimeToUtc('2026-01-15', '09:30', 'Europe/Berlin')).toBe(
    '2026-01-15T08:30:00.000Z',
  );
});
