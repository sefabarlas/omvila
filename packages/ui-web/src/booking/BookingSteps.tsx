import { cx } from './internal';
import shared from './shared.module.css';
import styles from './BookingSteps.module.css';

export const defaultBookingSteps = ['Hizmet', 'Personel', 'Saat', 'Onay'] as const;

export interface BookingStepsProps {
  /** Adım adları; personel seçimi kapalıysa "Personel" çıkarılır. Telefon doğrulaması Onay adımının içindedir. */
  steps?: readonly string[];
  /** Geçerli adımın sırası (0'dan). */
  current: number;
  className?: string;
}

/** Rezervasyon akışının ilerlemesi (components/BookingSteps). Geçerli adım aria-current="step". */
export function BookingSteps({
  steps = defaultBookingSteps,
  current,
  className,
}: BookingStepsProps) {
  return (
    <nav
      className={className}
      aria-label={`Rezervasyon adımları, adım ${String(current + 1)} / ${String(steps.length)}`}
    >
      <ol className={styles.steps}>
        {steps.map((label, index) => (
          <li
            key={label}
            className={cx(
              styles.step,
              index < current && styles.done,
              index === current && styles.current,
            )}
            aria-current={index === current ? 'step' : undefined}
          >
            <span className={styles.bar} aria-hidden="true" />
            <span className={styles.label}>
              {label}
              {index < current ? <span className={shared.visuallyHidden}>, tamamlandı</span> : null}
            </span>
          </li>
        ))}
      </ol>
    </nav>
  );
}
