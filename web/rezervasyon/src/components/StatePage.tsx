import type { ReactNode } from 'react';
import styles from './StatePage.module.css';

interface StatePageProps {
  /** Dekoratif ikon (lucide, 24 px); ekran okuyucudan gizlenir. */
  icon: ReactNode;
  title: string;
  description: string;
  /** Varsa tek eylem (bağlantı veya buton). */
  action?: ReactNode;
}

/**
 * Tam sayfa durum kartı (Durumlar.png "Bağlantı geçersiz" deseni): ikon kutusu, başlık, açıklama, tek eylem.
 * Sayfanın ana başlığıdır (h1); salon kartı olmayan sayfalarda kullanılır.
 */
export function StatePage({ icon, title, description, action }: StatePageProps) {
  return (
    <main className={styles.page}>
      <section className={styles.card} aria-labelledby="state-title">
        <span className={styles.icon} aria-hidden="true">
          {icon}
        </span>
        <h1 id="state-title" className={styles.title}>
          {title}
        </h1>
        <p className={styles.description}>{description}</p>
        {action ? <div className={styles.action}>{action}</div> : null}
      </section>
    </main>
  );
}
