import { expect, it } from 'vitest';
import { branchToday, daysBetween, fixedClock, toBranchDate } from './index';

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
