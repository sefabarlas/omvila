import { CalendarDays, CalendarX2 } from 'lucide-react';
import type { ReactNode } from 'react';
import { EmptyState } from '../panel/EmptyState';
import { classNames } from '../panel/Button';
import styles from './Calendar.module.css';

export interface CalendarClosedDayProps {
  /** Varsayılan "Kapalı gün". */
  title?: string;
  /** Neden ve ne yapılabileceği; varsayılan tasarımdaki metin. */
  description?: string;
  /** "Yalnızca bu gün için aç", "Çalışma saatleri" butonları. */
  actions?: ReactNode;
  headingLevel?: 2 | 3;
  className?: string;
}

const closedDescription =
  'Bu güne randevu alınmaz. Yine de çalışacaksanız günü yalnızca bu tarih için açabilirsiniz; haftalık düzen değişmez.';

/** Kapalı gün (salon-paneli/TakvimKapali): taralı kart, ızgara çizilmez. */
export function CalendarClosedDay({
  title = 'Kapalı gün',
  description = closedDescription,
  actions,
  headingLevel = 2,
  className,
}: CalendarClosedDayProps) {
  return (
    <section className={classNames(styles.notice, styles.closed, className)} aria-label={title}>
      <EmptyState
        className={classNames(styles.noticeInner)}
        icon={<CalendarX2 size={24} strokeWidth={1.75} />}
        title={title}
        description={description}
        actions={actions}
        headingLevel={headingLevel}
      />
    </section>
  );
}

export interface CalendarEmptyDayProps {
  /** "Pazartesi kapalısınız" gibi, veriden. */
  title: string;
  description?: string;
  /** "Özel saat ekle", "Salıya git". */
  actions?: ReactNode;
  headingLevel?: 2 | 3;
  className?: string;
}

/** Çalışma saati olmayan gün (salon-paneli/TakvimBos): düz kart, ızgara çizilmez. */
export function CalendarEmptyDay({
  title,
  description,
  actions,
  headingLevel = 2,
  className,
}: CalendarEmptyDayProps) {
  return (
    <section className={classNames(styles.notice, className)} aria-label={title}>
      <EmptyState
        className={classNames(styles.noticeInner)}
        icon={<CalendarDays size={24} strokeWidth={1.75} />}
        title={title}
        {...(description ? { description } : {})}
        actions={actions}
        headingLevel={headingLevel}
      />
    </section>
  );
}
