import type { ReactNode } from 'react';
import { classNames } from './Button';
import styles from './EmptyState.module.css';

export interface EmptyStateProps {
  icon?: ReactNode;
  /** Alanı adlandırır: "İlk hizmetinizi ekleyin". */
  title: string;
  /** Tek satır: neden önemli. */
  description?: string;
  /** Fiille başlayan buton(lar). */
  actions?: ReactNode;
  /** Başlık düzeyi (sayfadaki yerine göre). */
  headingLevel?: 2 | 3 | 4;
  className?: string;
}

/** Henüz verisi olmayan alanın davet ekranı; sahte sayı veya rozet içermez. */
export function EmptyState({
  icon,
  title,
  description,
  actions,
  headingLevel = 3,
  className,
}: EmptyStateProps) {
  const Heading = `h${String(headingLevel)}` as 'h2' | 'h3' | 'h4';
  return (
    <div className={classNames(styles.empty, className)}>
      {icon ? (
        <div className={styles.icon} aria-hidden="true">
          {icon}
        </div>
      ) : null}
      <Heading className={styles.title}>{title}</Heading>
      {description ? <p className={styles.description}>{description}</p> : null}
      {actions ? <div className={styles.actions}>{actions}</div> : null}
    </div>
  );
}
