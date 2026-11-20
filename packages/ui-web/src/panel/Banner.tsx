import { AlertTriangle, CheckCircle2, Info, XCircle } from 'lucide-react';
import type { ReactNode } from 'react';
import { classNames } from './Button';
import styles from './Banner.module.css';

export type BannerTone = 'bilgi' | 'uyari' | 'hata' | 'basari';

export interface BannerProps {
  tone?: BannerTone;
  title?: string;
  children?: ReactNode;
  /** Kullanıcının yapması gereken tek eylem ("Belgeleri yükle"). */
  action?: ReactNode;
  className?: string;
}

const icons = { bilgi: Info, uyari: AlertTriangle, hata: XCircle, basari: CheckCircle2 } as const;

/** Sayfa üstünde kalıcı durum mesajı. Hata şeridi ekran okuyucuya hemen okunur (role=alert). */
export function Banner({ tone = 'bilgi', title, children, action, className }: BannerProps) {
  const Icon = icons[tone];
  return (
    <div
      role={tone === 'hata' ? 'alert' : 'status'}
      className={classNames(styles.banner, styles[tone], className)}
    >
      <span className={styles.icon} aria-hidden="true">
        <Icon size={16} />
      </span>
      <div className={styles.text}>
        {title ? <strong className={styles.title}>{title}</strong> : null}
        {children}
      </div>
      {action ? <div className={styles.action}>{action}</div> : null}
    </div>
  );
}
