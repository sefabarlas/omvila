import type { ReactNode } from 'react';
import styles from './StateBanner.module.css';

interface StateBannerProps {
  /**
   * `hata`: saat doldu, kod deneme sınırı. `uyari`: kapora gelmedi, online randevu kapalı. `basari`: işlem
   * sonucu (saat değiştirildi).
   */
  tone: 'hata' | 'uyari' | 'basari';
  /** Dekoratif ikon (lucide, 20 px). */
  icon: ReactNode;
  title: string;
  description: string;
  /** Saat çipleri veya "Salonu ara" gibi eylemler. */
  children?: ReactNode;
}

/**
 * Akış içi durum kutusu (Durumlar.png: saat az önce doldu, SMS kodu deneme sınırı, kapora gelmedi,
 * online randevu kapalı). Ekran okuyucu açıldığında okur (role="status"; hata tonunda "alert").
 */
export function StateBanner({ tone, icon, title, description, children }: StateBannerProps) {
  return (
    <div
      className={[styles.banner, styles[tone]].join(' ')}
      role={tone === 'hata' ? 'alert' : 'status'}
    >
      <span className={styles.icon} aria-hidden="true">
        {icon}
      </span>
      <div className={styles.body}>
        <p className={styles.title}>{title}</p>
        <p className={styles.description}>{description}</p>
        {children ? <div className={styles.actions}>{children}</div> : null}
      </div>
    </div>
  );
}
