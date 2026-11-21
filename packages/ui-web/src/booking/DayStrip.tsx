import { useEffect, useRef } from 'react';
import type { ReactNode } from 'react';
import { formatDate } from '../format/date';
import { cx, useRovingRadio } from './internal';
import styles from './DayStrip.module.css';

export interface DayOption {
  /** Şubenin takvim günü, yyyy-aa-gg. */
  date: string;
  /** Uygun saati var: nokta gösterilir. Dolu gün seçilebilir (bekleme listesi). */
  available: boolean;
  /** Şube kapalı: devre dışı, "Kapalı" yazar. */
  closed?: boolean;
}

export interface DayStripProps {
  days: readonly DayOption[];
  value: string | null;
  onChange: (date: string) => void;
  /** Şubede bugün (branchToday): gün adı yerine "Bugün". */
  today?: string;
  /** Ay başlığı ("Kasım 2026", seçili günün ayı). Ertele ekranında gizlenir. */
  showMonth?: boolean;
  /** Başlığın sağındaki not: "60 gün ileriye kadar" (ayardan gelir). */
  hint?: ReactNode;
  /** Ay başlığının düzeyi. */
  headingLevel?: 2 | 3;
  /** Radyo grubunun ekran okuyucu adı. */
  label?: string;
  className?: string;
}

function dayLabel(day: DayOption, today?: string): string {
  const parts = [formatDate(day.date, 'long')];
  if (day.date === today) {
    parts.push('bugün');
  }

  if (day.closed) {
    parts.push('kapalı');
  } else if (!day.available) {
    parts.push('boş saat yok');
  }

  return parts.join(', ');
}

/**
 * Yatay gün şeridi (components/DayStrip): radyo grubu, oklarla gün değişir, kapalı günler atlanır.
 * Seçili gün görünür alana kaydırılır.
 */
export function DayStrip({
  days,
  value,
  onChange,
  today,
  showMonth = true,
  hint,
  headingLevel = 2,
  label = 'Gün',
  className,
}: DayStripProps) {
  const { itemProps } = useRovingRadio(
    days.map((day) => ({ value: day.date, disabled: day.closed })),
    value,
    onChange,
  );
  const stripRef = useRef<HTMLDivElement>(null);
  useEffect(() => {
    const strip = stripRef.current;
    const selected = strip?.querySelector<HTMLElement>('[aria-checked="true"]');
    if (!strip || !selected) {
      return;
    }

    // Yalnız şeridi yatay kaydırır; sayfa dikeyde oynamaz (şerit position: relative).
    const start = selected.offsetLeft;
    const end = start + selected.offsetWidth;
    if (start < strip.scrollLeft || end > strip.scrollLeft + strip.clientWidth) {
      strip.scrollLeft = Math.max(0, start - selected.offsetWidth);
    }
  }, [value]);

  const monthSource = value ?? days[0]?.date;
  const Heading = headingLevel === 2 ? 'h2' : 'h3';

  return (
    <div className={cx(styles.root, className)}>
      {showMonth && monthSource ? (
        <div className={styles.header}>
          <Heading className={styles.month}>{formatDate(monthSource, 'monthYear')}</Heading>
          {hint ? <span className={styles.hint}>{hint}</span> : null}
        </div>
      ) : null}
      <div ref={stripRef} className={styles.strip} role="radiogroup" aria-label={label}>
        {days.map((day) => {
          const isToday = day.date === today;
          return (
            <button
              key={day.date}
              type="button"
              className={cx(styles.day, day.date === value && styles.selected)}
              aria-label={dayLabel(day, today)}
              {...itemProps({ value: day.date, disabled: day.closed })}
            >
              <span className={styles.weekday}>
                {isToday ? 'Bugün' : formatDate(day.date, 'weekdayShort')}
              </span>
              <span className={styles.number}>{formatDate(day.date, 'day')}</span>
              {day.closed ? (
                <span className={styles.closed}>Kapalı</span>
              ) : (
                <span className={cx(styles.dot, day.available && styles.dotOn)} />
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
}
