import { statusLabels } from '../status';
import type { StatusKey } from '../status';
import { cx } from './internal';
import styles from './BookingStatusBadge.module.css';

export interface BookingStatusBadgeProps {
  /** Gösterim durumu (displayStatus); etiket sabit adlardan ("Onay bekliyor"). */
  status: StatusKey;
  className?: string;
}

/**
 * Mobil StatusBadge (components/StatusBadge; rezervasyon-sayfasi/Talep): renk her zaman durum adıyla, yalnız
 * renkle anlatılmaz. Panel StatusBadge'iyle aynı yedi durum ve renk; ölçü mobil ölçekten.
 */
export function BookingStatusBadge({ status, className }: BookingStatusBadgeProps) {
  return (
    <span className={cx(styles.badge, styles[status], className)}>
      <span className={styles.dot} aria-hidden="true" />
      {statusLabels[status]}
    </span>
  );
}
