import type { ReactNode } from 'react';
import { classNames } from './Button';
import styles from './Badge.module.css';

export type BadgeTone = 'notr' | 'marka' | 'vurgu' | 'basari' | 'uyari' | 'hata' | 'bilgi';

export interface BadgeProps {
  tone?: BadgeTone;
  /** Önde nokta. */
  dot?: boolean;
  /** Önde ikon (Lucide, 12 px): "Ödendi" ✓. */
  icon?: ReactNode;
  children: ReactNode;
  className?: string;
}

/** Kısa durum veya kategori etiketi; randevu durumu için StatusBadge. */
export function Badge({ tone = 'notr', dot = false, icon, children, className }: BadgeProps) {
  return (
    <span className={classNames(styles.badge, styles[tone], className)}>
      {dot ? <span className={styles.dot} aria-hidden="true" /> : null}
      {icon ? <span aria-hidden="true">{icon}</span> : null}
      {children}
    </span>
  );
}
