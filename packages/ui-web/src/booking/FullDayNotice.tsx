import { CalendarX } from 'lucide-react';
import type { ReactNode } from 'react';
import { BookingButton } from './BookingButton';
import { cx } from './internal';
import styles from './FullDayNotice.module.css';

export interface FullDayNoticeProps {
  /** "Perşembe dolu". */
  title: string;
  /** "Elif Kaya'nın 27 Kasım'da boş saati yok. En yakın uygun saat Cuma 10:30." */
  description?: ReactNode;
  /** En yakın uygun saate geçiş: { label: "Cuma 10:30'u seç" }. */
  action?: { label: string; onClick: () => void };
  /** Başlık düzeyi. */
  headingLevel?: 2 | 3;
  className?: string;
}

/** Dolu gün kartı (rezervasyon-sayfasi/DoluGun): ikon, başlık, açıklama, en yakın saate geçiş. */
export function FullDayNotice({
  title,
  description,
  action,
  headingLevel = 2,
  className,
}: FullDayNoticeProps) {
  const Heading = headingLevel === 2 ? 'h2' : 'h3';
  return (
    <section className={cx(styles.card, className)} aria-live="polite">
      <span className={styles.icon} aria-hidden="true">
        <CalendarX size={24} />
      </span>
      <Heading className={styles.title}>{title}</Heading>
      {description ? <p className={styles.description}>{description}</p> : null}
      {action ? (
        <BookingButton variant="secondary" size="md" onClick={action.onClick}>
          {action.label}
        </BookingButton>
      ) : null}
    </section>
  );
}
