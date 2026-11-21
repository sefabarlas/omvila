import { describe, expect, it } from 'vitest';
import type { CalendarView } from './api';
import {
  gridHours,
  summarize,
  toBlocks,
  unavailableRanges,
  visibleWeekDays,
  weekDates,
  weekLabel,
} from './calendar-view';

// Kural: panel bento blokları boşta 0 veya "—" (sahte sayı yok); ızgara şube saatleriyle; hafta pazartesi başlar.
const empty: CalendarView = {
  branchId: 'b',
  timeZone: 'Europe/Istanbul',
  from: '2026-11-25',
  to: '2026-11-25',
  staff: [],
  days: [],
  appointments: [],
  timeOffs: [],
};

const item = (
  id: string,
  start: string,
  status: CalendarView['appointments'][number]['status'],
) => ({
  appointmentId: id,
  itemId: `${id}-k`,
  staffId: 's1',
  startUtc: start,
  endUtc: new Date(Date.parse(start) + 45 * 60_000).toISOString(),
  status,
  displayStatus: status === 'Draft' ? ('PendingApproval' as const) : status,
  customerName: 'Selin Öztürk',
  serviceName: 'Saç kesimi',
  isRecurring: false,
  segments: [],
});

describe('özet blokları', () => {
  it('veri yokken sıradaki yok, sayılar 0', () => {
    expect(summarize(undefined, new Date('2026-11-25T09:00:00Z'))).toEqual({
      next: null,
      pending: { count: 0, expiresInMinutes: null },
      today: { appointments: 0, completed: 0 },
    });
  });

  it('çok kalemli randevu bir kez sayılır; geçmiş ve gelen sıradaki olmaz', () => {
    const view: CalendarView = {
      ...empty,
      staff: [
        {
          staffId: 's1',
          fullName: 'Elif Kaya',
          calendarColor: '#000',
          appointmentCount: 3,
          hasGapAvailability: false,
        },
      ],
      appointments: [
        item('a', '2026-11-25T07:00:00Z', 'Completed'),
        item('b', '2026-11-25T08:30:00Z', 'CheckedIn'),
        item('c', '2026-11-25T10:00:00Z', 'Confirmed'),
        { ...item('c', '2026-11-25T10:45:00Z', 'Confirmed'), itemId: 'c-2', serviceName: 'Fön' },
      ],
    };
    const summary = summarize(view, new Date('2026-11-25T09:00:00Z'));
    expect(summary.next).toEqual({
      inMinutes: 60,
      time: '13:00',
      line: 'Selin Öztürk · Saç kesimi · Elif',
    });
    expect(summary.today).toEqual({ appointments: 3, completed: 1 });
  });

  // acik-sorular.md T-72 (3): kapora bekleyen (status PendingApproval, displayStatus DepositPending) onay sayılmaz.
  it('"Onay bekliyor" gösterim durumuyla sayılır; kapora bekleyen ve süresi sayılmaz', () => {
    const view: CalendarView = {
      ...empty,
      appointments: [
        {
          ...item('p', '2026-11-25T13:00:00Z', 'PendingApproval'),
          holdExpiresUtc: '2026-11-25T10:35:00Z',
        },
        {
          ...item('k', '2026-11-25T11:30:00Z', 'PendingApproval'),
          displayStatus: 'DepositPending' as const,
          depositStatus: 'Awaiting' as const,
          holdExpiresUtc: '2026-11-25T09:20:00Z',
        },
      ],
    };
    expect(summarize(view, new Date('2026-11-25T09:00:00Z')).pending).toEqual({
      count: 1,
      expiresInMinutes: 95,
    });
  });

  it('havale bildirilen kapora bloğu "Havale bildirildi" işareti alır', () => {
    const view: CalendarView = {
      ...empty,
      appointments: [
        {
          ...item('k', '2026-11-25T11:30:00Z', 'PendingApproval'),
          displayStatus: 'DepositPending' as const,
          depositStatus: 'CustomerReported' as const,
        },
        {
          ...item('a', '2026-11-25T12:30:00Z', 'PendingApproval'),
          displayStatus: 'DepositPending' as const,
          depositStatus: 'Awaiting' as const,
        },
      ],
    };
    expect(toBlocks(view, true).map((block) => [block.status, block.note])).toEqual([
      ['kapora', 'Havale bildirildi'],
      ['kapora', undefined],
    ]);
  });
});

describe('ızgara', () => {
  it('şube saatleri, dışındaki randevuyu da kapsar', () => {
    const view: CalendarView = {
      ...empty,
      days: [
        {
          date: '2026-11-25',
          isClosed: false,
          branchIntervals: [{ start: '10:00', end: '19:00' }],
          staffHours: [],
        },
      ],
      appointments: [item('a', '2026-11-25T16:30:00Z', 'Confirmed')],
    };
    expect(gridHours(view, ['2026-11-25'])).toEqual({ openTime: '10:00', closeTime: '20:15' });
    expect(gridHours(empty, ['2026-11-25'])).toBeNull();
  });

  it('personelin çalışmadığı saatler ve izinler gölgeli', () => {
    const view: CalendarView = {
      ...empty,
      staff: [
        {
          staffId: 's1',
          fullName: 'Elif Kaya',
          calendarColor: '#000',
          appointmentCount: 0,
          hasGapAvailability: false,
        },
      ],
      days: [
        {
          date: '2026-11-25',
          isClosed: false,
          staffHours: [
            {
              staffId: 's1',
              intervals: [
                { start: '10:00', end: '13:00' },
                { start: '13:45', end: '18:00' },
              ],
            },
          ],
        },
      ],
      timeOffs: [
        {
          id: 't',
          staffId: 's1',
          type: 'Leave',
          startUtc: '2026-11-25T14:00:00Z',
          endUtc: '2026-11-25T15:00:00Z',
          isAllDay: false,
        },
      ],
    };
    expect(
      unavailableRanges(view, ['2026-11-25'], { openTime: '09:00', closeTime: '19:00' }),
    ).toEqual([
      { staffId: 's1', startUtc: '2026-11-25T06:00:00.000Z', endUtc: '2026-11-25T07:00:00.000Z' },
      {
        staffId: 's1',
        startUtc: '2026-11-25T10:00:00.000Z',
        endUtc: '2026-11-25T10:45:00.000Z',
        label: 'Mola',
      },
      { staffId: 's1', startUtc: '2026-11-25T15:00:00.000Z', endUtc: '2026-11-25T16:00:00.000Z' },
      {
        staffId: 's1',
        startUtc: '2026-11-25T14:00:00Z',
        endUtc: '2026-11-25T15:00:00Z',
        label: 'İzin',
      },
    ]);
  });
});

describe('hafta', () => {
  it('pazartesiden pazara; kapalı gün gizlenir; etiket', () => {
    const dates = weekDates('2026-11-25');
    expect(dates[0]).toBe('2026-11-23');
    expect(dates.at(-1)).toBe('2026-11-29');
    const view: CalendarView = {
      ...empty,
      days: dates.map((date) => ({ date, isClosed: date === '2026-11-29', staffHours: [] })),
    };
    expect(visibleWeekDays(view, dates)).toHaveLength(6);
    expect(weekLabel(visibleWeekDays(view, dates))).toBe('23–28 Kasım');
    expect(weekLabel(weekDates('2026-12-01'))).toBe('30 Kasım–6 Aralık');
  });
});
