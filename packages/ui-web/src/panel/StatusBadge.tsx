import { statusLabels } from '../status';
import type { StatusKey } from '../status';
import { classNames } from './Button';
import styles from './Badge.module.css';

export interface StatusBadgeProps {
  status: StatusKey;
  className?: string;
}

/** Randevu durumunun tek gösterimi: renk her zaman durum adıyla (yalnız renkle anlatılmaz). */
export function StatusBadge({ status, className }: StatusBadgeProps) {
  return (
    <span className={classNames(styles.badge, styles[status], className)}>
      <span className={styles.dot} aria-hidden="true" />
      {statusLabels[status]}
    </span>
  );
}
