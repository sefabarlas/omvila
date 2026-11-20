import { expect, it } from 'vitest';
import {
  branchHour,
  branchToday,
  daysBetween,
  fixedClock,
  localDateTimeToUtc,
  toBranchDate,
} from './index';

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
