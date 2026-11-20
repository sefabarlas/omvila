import { describe, expect, it } from 'vitest';
import {
  dayError,
  groupOverrides,
  overrideKindLabel,
  suggestedIntervals,
  toDrafts,
  toWeeklyHours,
} from './hours';
import type { DayDraft, WorkingHoursOverride } from './hours';

// Kural: dilim-01 kabul kriteri 5 (Salı 10:00–13:00 ve 13:45–19:00; örtüşen ve gece yarısını aşan aralık hatalı),
// kabul kriteri 6 (1 Ocak tek gün, bayram 3 gün), WeeklyHours (listede olmayan gün kapalı).
const day = (intervals: [string, string][], open = true): DayDraft => ({
  dayOfWeek: 2,
  open,
  intervals: intervals.map(([start, end]) => ({ start, end })),
});

describe('haftalık saatler', () => {
  it('sunucu saatleri yedi güne açılır, kapalı gün gönderilmez, aralıklar sıralanır', () => {
    const drafts = toDrafts({
      days: [{ dayOfWeek: 2, intervals: [{ start: '10:00', end: '13:00' }] }],
    });
    expect(drafts).toHaveLength(7);
    expect(drafts[0]).toEqual({ dayOfWeek: 1, open: false, intervals: [] });
    expect(drafts[1]?.open).toBe(true);
    drafts[1]?.intervals.unshift({ start: '13:45', end: '19:00' });
    expect(toWeeklyHours(drafts)).toEqual({
      days: [
        {
          dayOfWeek: 2,
          intervals: [
            { start: '10:00', end: '13:00' },
            { start: '13:45', end: '19:00' },
          ],
        },
      ],
    });
  });

  it('Salı 10:00–13:00 ve 13:45–19:00 geçerli; örtüşen, ters ve eksik aralık hatalı', () => {
    expect(
      dayError(
        day([
          ['10:00', '13:00'],
          ['13:45', '19:00'],
        ]),
      ),
    ).toBeUndefined();
    expect(
      dayError(
        day([
          ['10:00', '14:00'],
          ['13:45', '19:00'],
        ]),
      ),
    ).toBe('Aralıklar örtüşmemeli.');
    expect(dayError(day([['22:00', '02:00']]))).toContain('gece yarısını aşamaz');
    expect(dayError(day([['10:00', '']]))).toBe('Başlangıç ve bitiş saatini girin.');
    expect(dayError(day([], false))).toBeUndefined();
  });

  it('açılan güne önceki açık günün saatleri önerilir; hiç yoksa boş aralık', () => {
    const drafts = toDrafts({
      days: [{ dayOfWeek: 2, intervals: [{ start: '10:00', end: '20:00' }] }],
    });
    expect(suggestedIntervals(drafts, 3)).toEqual([{ start: '10:00', end: '20:00' }]);
    expect(suggestedIntervals(drafts, 1)).toEqual([{ start: '10:00', end: '20:00' }]);
    expect(suggestedIntervals(toDrafts({ days: [] }), 1)).toEqual([{ start: '', end: '' }]);
  });
});

describe('özel günler', () => {
  const closed = (id: string, date: string, note: string): WorkingHoursOverride => ({
    id,
    date,
    kind: 'Closure',
    isClosed: true,
    intervals: [],
    note,
  });

  it('ardışık ve aynı kuraldaki günler tek satır; ayrı günler ayrı', () => {
    const groups = groupOverrides([
      closed('c', '2027-03-21', 'Ramazan Bayramı'),
      closed('a', '2027-01-01', 'Yılbaşı'),
      closed('b', '2027-03-20', 'Ramazan Bayramı'),
      closed('d', '2027-03-22', 'Ramazan Bayramı'),
      closed('e', '2027-03-23', 'Tadilat'),
    ]);
    expect(groups.map((g) => [g.fromDate, g.toDate, g.dayCount, g.ids])).toEqual([
      ['2027-01-01', '2027-01-01', 1, ['a']],
      ['2027-03-20', '2027-03-22', 3, ['b', 'c', 'd']],
      ['2027-03-23', '2027-03-23', 1, ['e']],
    ]);
  });

  it('tür etiketleri', () => {
    expect(overrideKindLabel({ ...closed('a', '2027-01-01', ''), kind: 'Holiday' })).toBe(
      'Resmi tatil · kapalı',
    );
    expect(
      overrideKindLabel({ ...closed('a', '2026-12-31', ''), kind: 'ShortDay', isClosed: false }),
    ).toBe('Özel saat');
  });
});
