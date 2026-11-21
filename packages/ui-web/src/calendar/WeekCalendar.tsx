import { formatDate } from '../format';
import { classNames } from '../panel/Button';
import { toBranchDate } from '../time';
import { TimeGrid } from './TimeGrid';
import type { GridColumn } from './TimeGrid';
import type { CalendarCommonProps, CalendarStaff } from './types';
import styles from './Calendar.module.css';

export interface WeekCalendarProps extends CalendarCommonProps {
  /** Haftası gösterilen personel. */
  staff: Pick<CalendarStaff, 'id' | 'name'>;
  /** Şubenin yerel takvim günleri (genelde 6–7 gün): ["2026-11-24", …]. */
  days: readonly string[];
}

/**
 * Haftalık takvim (WeekCalendar): tek personelin haftası, her gün bir kolon. Bugün petrol daire içinde; şu anki
 * saat çizgisi yalnız bugünün kolonunda. Sürükleyince gün ve saat değişir, personel aynı kalır.
 */
export function WeekCalendar({ staff, days, label, now, timeZone, ...common }: WeekCalendarProps) {
  const today = now ? toBranchDate(now, timeZone) : null;
  const columns: GridColumn[] = days.map((date) => {
    const isToday = date === today;
    return {
      key: date,
      staffId: staff.id,
      date,
      name: formatDate(date, 'panel'),
      header: (
        <span className={classNames(styles.dayHead, isToday && styles.today)}>
          <small aria-hidden="true">{formatDate(date, 'weekdayShort')}</small>
          <strong aria-hidden="true">{date.slice(8).replace(/^0/, '')}</strong>
          <span className={styles.srOnly}>
            {formatDate(date, 'panel')}
            {isToday ? ', bugün' : ''}
          </span>
        </span>
      ),
    };
  });

  return (
    <TimeGrid
      {...common}
      {...(now ? { now } : {})}
      timeZone={timeZone}
      label={label ?? `${staff.name} haftalık takvim`}
      columns={columns}
      headClassName={classNames(styles.weekHead)}
    />
  );
}
